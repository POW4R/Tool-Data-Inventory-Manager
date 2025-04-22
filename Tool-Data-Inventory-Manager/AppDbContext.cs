
using Microsoft.EntityFrameworkCore;

namespace Tool_Data_Inventory_Manager
{
    internal class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Machine_Product> Machine_Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Inventory Manager;Trusted_Connection=True;Encrypt=False;");
        }
    }
}
