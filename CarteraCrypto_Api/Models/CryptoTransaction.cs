using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarteraCrypto_Api.Models
{
    public class CryptoTransaction
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string cryptoCode { get; set; }

        [Required]
        public string action { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,8)")]
        public decimal cryptoAmount { get; set; }

        [Required]
        public int clientId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal money { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime datetime { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
    }
}
