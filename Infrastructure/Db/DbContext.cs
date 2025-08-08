using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entity;

namespace MinimalApi.Infrastructure.Db
{
      public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<Administrator> Administrators { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConnection = _configuration.GetConnectionString("MyPostgres").ToString();

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(stringConnection);
            }
        }
    }
}
