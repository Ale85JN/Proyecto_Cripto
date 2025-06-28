using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class TransactionDto
    {
        public int id { get; set; }
        public string crypto_Code { get; set; }

       
        public string action { get; set; }

       
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal crypto_Amount { get; set; }

        public decimal money { get; set; }
        public int clientId { get; set; }

        public string clientName { get; set; }

        public string datetime { get; set; }
    }
}
