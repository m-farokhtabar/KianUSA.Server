using KianUSA.Application.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KianUSA.Application.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired(true).HasMaxLength(200);
            builder.Property(x => x.Password).IsRequired(true).HasMaxLength(250);
            builder.Property(x => x.Name).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.LastName).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.StoreName).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.Security).IsRequired(false).HasMaxLength(500);
            
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
