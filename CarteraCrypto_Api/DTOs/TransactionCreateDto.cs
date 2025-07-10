using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class TransactionCreateDto
    {
        [Required]
        public string cryptoCode { get; set; }
        [Required]
        [RegularExpression("^(purchase|sale)$", ErrorMessage = "The action must be 'purchase' or 'sale'.")]
        public string action { get; set; }
        [Required]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal cryptoAmount { get; set; }
        [Required]
        public int clientId { get; set; }
        [Required]
        public string datetime { get; set; }
    }
}
