using EquityPositions.Api.DTOs;
using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Enums;
using EquityPositions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EquityPositions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPositionCalculator _positionCalculator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            ITransactionRepository transactionRepository,
            IPositionCalculator positionCalculator,
            ILogger<TransactionsController> logger)
        {
            _transactionRepository = transactionRepository;
            _positionCalculator = positionCalculator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransactionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var response = transactions.Select(MapToResponse);
            return Ok(ApiResponse<IEnumerable<TransactionResponse>>.SuccessResponse(response));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TransactionResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            
            if (transaction == null)
            {
                return NotFound(ApiResponse<TransactionResponse>.ErrorResponse($"Transaction with ID {id} not found"));
            }

            return Ok(ApiResponse<TransactionResponse>.SuccessResponse(MapToResponse(transaction)));
        }

        [HttpGet("trade/{tradeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransactionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByTradeId(int tradeId)
        {
            var transactions = await _transactionRepository.GetByTradeIdAsync(tradeId);
            var response = transactions.Select(MapToResponse);
            return Ok(ApiResponse<IEnumerable<TransactionResponse>>.SuccessResponse(response));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TransactionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<TransactionResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<TransactionResponse>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var transaction = new Transaction
                {
                    TradeId = request.TradeId,
                    Version = request.Version,
                    SecurityCode = request.SecurityCode.ToUpperInvariant(),
                    Quantity = request.Quantity,
                    Action = ParseAction(request.Action),
                    Side = ParseSide(request.Side),
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false
                };

                var createdTransaction = await _transactionRepository.AddAsync(transaction);

                await _positionCalculator.ProcessTransactionAsync(createdTransaction);
                
                createdTransaction.IsProcessed = true;
                await _transactionRepository.UpdateAsync(createdTransaction);

                _logger.LogInformation(
                    "Transaction {TransactionId} processed: TradeId={TradeId}, Action={Action}, Security={SecurityCode}, Quantity={Quantity}",
                    createdTransaction.TransactionId,
                    createdTransaction.TradeId,
                    createdTransaction.Action,
                    createdTransaction.SecurityCode,
                    createdTransaction.Quantity);

                var response = MapToResponse(createdTransaction);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = createdTransaction.TransactionId }, 
                    ApiResponse<TransactionResponse>.SuccessResponse(response, "Transaction created and processed successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while processing transaction");
                return BadRequest(ApiResponse<TransactionResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction");
                return StatusCode(500, ApiResponse<TransactionResponse>.ErrorResponse("An error occurred while processing the transaction"));
            }
        }

        private static TransactionAction ParseAction(string action)
        {
            return action.ToUpperInvariant() switch
            {
                "INSERT" => TransactionAction.Insert,
                "UPDATE" => TransactionAction.Update,
                "CANCEL" => TransactionAction.Cancel,
                _ => throw new ArgumentException($"Invalid action: {action}")
            };
        }

        private static TradeSide ParseSide(string side)
        {
            return side.ToLowerInvariant() switch
            {
                "buy" => TradeSide.Buy,
                "sell" => TradeSide.Sell,
                _ => throw new ArgumentException($"Invalid side: {side}")
            };
        }

        private static TransactionResponse MapToResponse(Transaction transaction)
        {
            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                TradeId = transaction.TradeId,
                Version = transaction.Version,
                SecurityCode = transaction.SecurityCode,
                Quantity = transaction.Quantity,
                Action = transaction.Action.ToString().ToUpperInvariant(),
                Side = transaction.Side.ToString(),
                CreatedAt = transaction.CreatedAt,
                IsProcessed = transaction.IsProcessed
            };
        }
    }
}

