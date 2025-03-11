
using Microsoft.EntityFrameworkCore;

namespace Tool_Data_Inventory_Manager
{
    internal class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=YOUR_SERVER_NAME;Database=UserDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
