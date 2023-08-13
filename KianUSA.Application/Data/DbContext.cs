using KianUSA.Application.Data.Config;
using KianUSA.Application.Entity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace KianUSA.Application.Data
{
    public class Context : DbContext
    {
        public static string ConnectionString { get; set; } = "Host=localhost;Port=5433;Database=KianUsa;Username=admin;Password=123456";
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryCategory> CategoryCategories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<PoData> PoDatas { get; set; }

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