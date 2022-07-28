using KianUSA.Application.Data.Config;
using KianUSA.Application.Entity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace KianUSA.Application.Data
{
    public class Context : DbContext
    {
        public static string ConnectionString { get; set; } = "Host=localhost;Port=49153;Database=KianUsa;Username=postgres;Password=postgrespw";
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryCategory> CategoryCategories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                        
            optionsBuilder.UseNpgsql(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}