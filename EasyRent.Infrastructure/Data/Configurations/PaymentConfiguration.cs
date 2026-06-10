using EasyRent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyRent.Infrastructure.Data.Configurations;

/// <summary>Schema configuration for <see cref="Payment"/>.</summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Method).IsRequired().HasMaxLength(50);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Booking 1 ── 1 Payment. The foreign key lives on Payment; deleting a booking
        // cascades to its single payment.
        b.HasOne(x => x.Booking)
            .WithOne(bk => bk.Payment)
            .HasForeignKey<Payment>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // One payment per booking.
        b.HasIndex(x => x.BookingId).IsUnique();
    }
}
