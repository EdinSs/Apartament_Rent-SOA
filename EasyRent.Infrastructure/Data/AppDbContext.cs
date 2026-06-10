using System.Reflection;
using EasyRent.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyRent.Infrastructure.Data;

/// <summary>
/// The EF Core database context. By inheriting <see cref="IdentityDbContext{TUser}"/> we get
/// the full ASP.NET Identity schema (users, roles, claims, tokens) for free, and we add the
/// EasyRent domain tables (apartments, bookings, payments, reviews) on top.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Call base first so the Identity tables are configured before our entities.
        base.OnModelCreating(builder);

        // Pick up every IEntityTypeConfiguration<> in this assembly (Data/Configurations/*).
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
