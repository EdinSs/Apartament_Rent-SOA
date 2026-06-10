using EasyRent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyRent.Infrastructure.Data.Configurations;

/// <summary>Schema configuration for <see cref="Review"/>.</summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Comment).HasMaxLength(1000);
        b.Property(x => x.TenantId).IsRequired();

        // Apartment 1 ── * Review.
        // Cascade: deleting an apartment removes its reviews (single cascade path into Review).
        // WithMany() with no argument because Apartment has no Reviews navigation (one-directional).
        b.HasOne(x => x.Apartment)
            .WithMany()
            .HasForeignKey(x => x.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tenant (ApplicationUser) 1 ── * Review.
        // Restrict: avoids a second cascade path into Review.
        b.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
