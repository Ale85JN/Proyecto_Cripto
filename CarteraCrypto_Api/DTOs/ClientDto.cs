using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class ClientDto
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }

        [Required]
        public string email { get; set; }
    }
}
