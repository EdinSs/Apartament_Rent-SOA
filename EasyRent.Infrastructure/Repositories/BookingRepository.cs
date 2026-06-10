using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Domain.Entities;
using EasyRent.Domain.Enums;
using EasyRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyRent.Infrastructure.Repositories;

/// <summary>EF Core data access for bookings, including the date-overlap availability check.</summary>
public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    /// <summary>
    /// Overridden to eager-load the Apartment (and tracked, not AsNoTracking) so the service
    /// can verify landlord ownership and then mutate the booking's status in the same unit of work.
    /// </summary>
    public override async Task<Booking?> GetByIdAsync(int id) =>
        await Context.Bookings
            .Include(b => b.Apartment)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<bool> HasOverlapAsync(int apartmentId, DateTime checkIn, DateTime checkOut) =>
        await Context.Bookings.AnyAsync(b =>
            b.ApartmentId == apartmentId &&
            (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Paid) &&
            b.CheckIn < checkOut && checkIn < b.CheckOut);

    public async Task<IEnumerable<Booking>> GetByTenantAsync(string tenantId) =>
        await Context.Bookings.AsNoTracking()
            .Include(b => b.Apartment)
            .Where(b => b.TenantId == tenantId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Booking>> GetIncomingForLandlordAsync(string landlordId) =>
        await Context.Bookings.AsNoTracking()
            .Include(b => b.Apartment)
            .Where(b => b.Apartment!.LandlordId == landlordId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
}
