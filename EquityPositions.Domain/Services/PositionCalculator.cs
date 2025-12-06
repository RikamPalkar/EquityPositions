using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Enums;
using EquityPositions.Domain.Interfaces;

namespace EquityPositions.Domain.Services
{
    public class PositionCalculator : IPositionCalculator
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ITradeStateRepository _tradeStateRepository;

        public PositionCalculator(
            IPositionRepository positionRepository,
            ITradeStateRepository tradeStateRepository)
        {
            _positionRepository = positionRepository;
            _tradeStateRepository = tradeStateRepository;
        }

        public async Task<Position> ProcessTransactionAsync(Transaction transaction)
        {
            var existingTradeState = await _tradeStateRepository.GetByTradeIdAsync(transaction.TradeId);

            return transaction.Action switch
            {
                TransactionAction.Insert => await ProcessInsertAsync(transaction, existingTradeState),
                TransactionAction.Update => await ProcessUpdateAsync(transaction, existingTradeState),
                TransactionAction.Cancel => await ProcessCancelAsync(transaction, existingTradeState),
                _ => throw new ArgumentException($"Unknown transaction action: {transaction.Action}")
            };
        }

        private async Task<Position> ProcessInsertAsync(Transaction transaction, TradeState? existingTradeState)
        {
            if (existingTradeState != null)
            {
                throw new InvalidOperationException(
                    $"Trade {transaction.TradeId} already exists. Cannot insert duplicate trade.");
            }

            var quantityDelta = CalculateQuantityDelta(transaction.Quantity, transaction.Side);

            var tradeState = new TradeState
            {
                TradeId = transaction.TradeId,
                CurrentVersion = transaction.Version,
                SecurityCode = transaction.SecurityCode,
                Quantity = transaction.Quantity,
                Side = transaction.Side,
                IsCancelled = false
            };

            await _tradeStateRepository.AddAsync(tradeState);
            return await _positionRepository.AddOrUpdateAsync(transaction.SecurityCode, quantityDelta);
        }

        private async Task<Position> ProcessUpdateAsync(Transaction transaction, TradeState? existingTradeState)
        {
            if (existingTradeState == null)
            {
                throw new InvalidOperationException(
                    $"Trade {transaction.TradeId} does not exist. Cannot update non existent trade.");
            }

            if (existingTradeState.IsCancelled)
            {
                throw new InvalidOperationException(
                    $"Trade {transaction.TradeId} has been cancelled. Cannot update cancelled trade.");
            }

            if (transaction.Version <= existingTradeState.CurrentVersion)
            {
                throw new InvalidOperationException(
                    $"Transaction version {transaction.Version} is not greater than current version {existingTradeState.CurrentVersion}.");
            }

            var previousQuantityDelta = CalculateQuantityDelta(existingTradeState.Quantity, existingTradeState.Side);
            await _positionRepository.AddOrUpdateAsync(existingTradeState.SecurityCode, -previousQuantityDelta);

            var newQuantityDelta = CalculateQuantityDelta(transaction.Quantity, transaction.Side);
            var position = await _positionRepository.AddOrUpdateAsync(transaction.SecurityCode, newQuantityDelta);

            existingTradeState.CurrentVersion = transaction.Version;
            existingTradeState.SecurityCode = transaction.SecurityCode;
            existingTradeState.Quantity = transaction.Quantity;
            existingTradeState.Side = transaction.Side;
            existingTradeState.LastUpdatedAt = DateTime.UtcNow;

            await _tradeStateRepository.UpdateAsync(existingTradeState);

            return position;
        }

        private async Task<Position> ProcessCancelAsync(Transaction transaction, TradeState? existingTradeState)
        {
            if (existingTradeState == null)
            {
                throw new InvalidOperationException(
                    $"Trade {transaction.TradeId} does not exist. Cannot cancel non existent trade.");
            }

            if (existingTradeState.IsCancelled)
            {
                throw new InvalidOperationException(
                    $"Trade {transaction.TradeId} has already been cancelled.");
            }

            var previousQuantityDelta = CalculateQuantityDelta(existingTradeState.Quantity, existingTradeState.Side);
            var position = await _positionRepository.AddOrUpdateAsync(existingTradeState.SecurityCode, -previousQuantityDelta);

            existingTradeState.IsCancelled = true;
            existingTradeState.CurrentVersion = transaction.Version;
            existingTradeState.LastUpdatedAt = DateTime.UtcNow;

            await _tradeStateRepository.UpdateAsync(existingTradeState);

            return position;
        }

        private static int CalculateQuantityDelta(int quantity, TradeSide side)
        {
            return side == TradeSide.Buy ? quantity : -quantity;
        }
    }
}
