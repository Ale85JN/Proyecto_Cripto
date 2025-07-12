using System.ComponentModel.DataAnnotations;

namespace CarteraCrypto_Api.DTOs
{
    public class ClientDto
    {
        public int id { get; set; }
       
        public string name { get; set; }

        
        public string email { get; set; }
    }
}
