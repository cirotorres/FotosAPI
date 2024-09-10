using FotosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FotosAPI.Data
{
    public class ConnectionContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }

        // Configuração para o PostgreSQL
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          => optionsBuilder.UseNpgsql(
              "Server=localhost;" +
              "Port=5432;Database=postgres;" +
              "User Id=postgres;" +
              "Password=1234;");
    }
}
