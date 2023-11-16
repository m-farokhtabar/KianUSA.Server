using KianUSA.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KianUSA.Application.Data.Config
{
    public class GroupConfig : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable(nameof(Group));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(200);
            builder.Property(x => x.IsVisible).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.Order).IsRequired(true).HasDefaultValue(0);

            builder.HasIndex(x => x.Name).IsUnique();            
        }
    }
}
