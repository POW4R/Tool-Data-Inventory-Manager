
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace Tool_Data_Inventory_Manager
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Machine> Machines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("SQL_SERVER"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Tools)
                .WithMany()
                .UsingEntity(j => j.ToTable("ProductTools"));

            modelBuilder.Entity<Tool>()
                .Property(t => t.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            modelBuilder.Entity<Machine>()
                .HasMany(m => m.Products)
                .WithMany();
        }
    }
}
