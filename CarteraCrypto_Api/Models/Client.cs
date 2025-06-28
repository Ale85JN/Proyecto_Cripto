using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.Models
{
    public class Client
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string name { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string email { get; set; }
    }
}
