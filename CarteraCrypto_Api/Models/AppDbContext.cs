using Microsoft.EntityFrameworkCore;

namespace CarteraCrypto_Api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<CryptoTransaction> Transactions { get; set; }
    }
}
