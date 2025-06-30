using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class TransactionUpdateDto
    {
        [RegularExpression("^[a-zA-Z]{3,10}$", ErrorMessage ="Codigo de Criptomoneda invalido")]
        public string cryptoCode { get; set; }
        [RegularExpression("^(purchase| sale)$", ErrorMessage = "La accion debe ser 'purchase' o 'sale'.")]
        public string action { get; set; }
        [Range(0.00000001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal? cryptoAmount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal? money { get; set; }
        public int? clientId { get; set; }
        public string datetime { get; set; }


    }
}
