using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.Models
{
    public class Client
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string email { get; set; }
    }
}
