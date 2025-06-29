namespace CarteraCrypto_Api.DTOs
{
    public class TransactionResponseDto
    {
        public int id { get; set; }
        public string cryptoCode { get; set; }
        public string action { get; set; }
        public decimal cryptoAmount { get; set; }
        public decimal money { get; set; }
        public DateTime datetime { get; set; }

        public int clientId { get; set; }
        public string clientName { get; set; }
    }
}
