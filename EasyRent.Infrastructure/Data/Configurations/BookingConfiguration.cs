using EasyRent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyRent.Infrastructure.Data.Configurations;

/// <summary>Schema configuration for <see cref="Booking"/>.</summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.TotalPrice).HasPrecision(18, 2);
        b.Property(x => x.TenantId).IsRequired();

        // Store the enum as a readable string ("Pending", "Approved", …) instead of an int,
        // so the database is human-inspectable and resilient to enum reordering.
        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Apartment 1 ── * Booking.
        // Cascade: deleting an apartment removes its bookings (this is the single cascade
        // path into Booking).
        b.HasOne(x => x.Apartment)
            .WithMany(a => a.Bookings)
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tenant (ApplicationUser) 1 ── * Booking.
        // Restrict: avoids a SECOND cascade path into Booking and preserves booking history.
        b.HasOne(x => x.Tenant)
            .WithMany(u => u.Bookings)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
