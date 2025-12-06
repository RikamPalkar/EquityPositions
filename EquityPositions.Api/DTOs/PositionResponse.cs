namespace EquityPositions.Api.DTOs
{
    public class PositionResponse
    {
        public string SecurityCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}

