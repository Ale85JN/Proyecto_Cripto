using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class TransactionUpdateDto
    {
        [RegularExpression("^[a-zA-Z]{3,10}$", ErrorMessage ="Invalid Crypto code")]
        public string? cryptoCode { get; set; }
        [RegularExpression("^(purchase| sale)$", ErrorMessage = "The action must be 'purchase' or 'sale'.")]
        public string? action { get; set; }
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? cryptoAmount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? money { get; set; }
        public int? clientId { get; set; }
        public string? datetime { get; set; }


    }
}
