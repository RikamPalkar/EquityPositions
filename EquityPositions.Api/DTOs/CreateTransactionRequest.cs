using System.ComponentModel.DataAnnotations;

namespace EquityPositions.Api.DTOs
{
    public class CreateTransactionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TradeId must be a positive integer")]
        public int TradeId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Version must be a positive integer")]
        public int Version { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "SecurityCode must be between 1 and 20 characters")]
        public string SecurityCode { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer")]
        public int Quantity { get; set; }

        [Required]
        [RegularExpression("^(INSERT|UPDATE|CANCEL)$", ErrorMessage = "Action must be INSERT, UPDATE, or CANCEL")]
        public string Action { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Buy|Sell)$", ErrorMessage = "Side must be Buy or Sell")]
        public string Side { get; set; } = string.Empty;
    }
}

