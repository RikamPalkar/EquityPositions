using EquityPositions.Domain.Enums;

namespace EquityPositions.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int TradeId { get; set; }
        public int Version { get; set; }
        public string SecurityCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public TransactionAction Action { get; set; }
        public TradeSide Side { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; }
    }
}

