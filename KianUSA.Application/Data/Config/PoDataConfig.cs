using KianUSA.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KianUSA.Application.Data.Config
{
    public class PoDataConfig : IEntityTypeConfiguration<PoData>
    {
        public void Configure(EntityTypeBuilder<PoData> builder)
        {
            builder.ToTable(nameof(PoData));
            builder.HasKey(x => x.PoNumber);
            builder.Property(x => x.PoNumber).HasMaxLength(10);
            builder.Property(x => x.FactoryStatus).IsRequired(false);
            builder.Property(x => x.StatusDate).IsRequired(false);
            builder.Property(x => x.FactoryContainerNumber).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.BookingDate).IsRequired(false);
            builder.Property(x => x.DocumentsSendOutDate).IsRequired(false);

            builder.Property(x => x.ForwarderName).IsRequired(false);
            builder.Property(x => x.FactoryBookingDate).IsRequired(false);
            builder.Property(x => x.Rate).IsRequired(false);
            builder.Property(x => x.ETD).IsRequired(false);
            builder.Property(x => x.ETA).IsRequired(false);
            builder.Property(x => x.PortOfDischarge).IsRequired(false);
            builder.Property(x => x.DischargeStatus).IsRequired(false);

            builder.Property(x => x.ShippmentStatus).IsRequired(false);
            builder.Property(x => x.ConfirmDate).IsRequired(false);

            builder.Property(x => x.GateIn).IsRequired(false);
            builder.Property(x => x.EmptyDate).IsRequired(false);
            builder.Property(x => x.GateOut).IsRequired(false);
            builder.Property(x => x.BillDate).IsRequired(false);
        }
    }
}
