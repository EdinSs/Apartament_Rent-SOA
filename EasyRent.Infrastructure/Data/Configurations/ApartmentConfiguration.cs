using EasyRent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyRent.Infrastructure.Data.Configurations;

/// <summary>Schema configuration for <see cref="Apartment"/>.</summary>
public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> b)
    {
        b.HasKey(a => a.Id);

        b.Property(a => a.Title).IsRequired().HasMaxLength(150);
        b.Property(a => a.Description).HasMaxLength(2000);
        b.Property(a => a.Address).IsRequired().HasMaxLength(250);
        b.Property(a => a.City).IsRequired().HasMaxLength(100);
        b.Property(a => a.PhotoUrl).HasMaxLength(500);
        b.Property(a => a.PricePerNight).HasPrecision(18, 2); // money: avoids float rounding
        b.Property(a => a.LandlordId).IsRequired();

        // Landlord (ApplicationUser) 1 ── * Apartment.
        // Restrict: you cannot delete a landlord who still owns listings. This protects data
        // AND prevents a cascade path into Booking that would clash with Apartment→Booking.
        b.HasOne(a => a.Landlord)
            .WithMany(u => u.Apartments)
            .HasForeignKey(a => a.LandlordId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(a => a.City); // City is the most common search filter.
    }
}
