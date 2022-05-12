using KianUSA.Application.Data.Config;
using KianUSA.Application.Entity;
using Microsoft.EntityFrameworkCore;

namespace KianUSA.Application.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source =.;Initial Catalog = KianUsa;Integrated Security=true;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}