namespace EquityPositions.Domain.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string SecurityCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

