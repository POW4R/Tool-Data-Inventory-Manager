
using Microsoft.EntityFrameworkCore;

namespace Tool_Data_Inventory_Manager
{
    internal class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;Encrypt=False;");
        }
    }
}
