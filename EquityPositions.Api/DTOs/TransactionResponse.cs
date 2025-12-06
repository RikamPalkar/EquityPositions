namespace EquityPositions.Api.DTOs
{
    public class TransactionResponse
    {
        public int TransactionId { get; set; }
        public int TradeId { get; set; }
        public int Version { get; set; }
        public string SecurityCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Side { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsProcessed { get; set; }
    }
}

