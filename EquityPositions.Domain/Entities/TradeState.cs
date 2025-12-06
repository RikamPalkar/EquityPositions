using EquityPositions.Domain.Enums;

namespace EquityPositions.Domain.Entities
{
    /// <summary>
    /// Tracks the current state of a trade to handle version management
    /// and out of order transaction processing
    /// </summary>
    public class TradeState
    {
        public int Id { get; set; }
        public int TradeId { get; set; }
        public int CurrentVersion { get; set; }
        public string SecurityCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public TradeSide Side { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

