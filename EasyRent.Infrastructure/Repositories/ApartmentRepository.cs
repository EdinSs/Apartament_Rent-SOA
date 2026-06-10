using EasyRent.Application.DTOs.Apartments;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Domain.Entities;
using EasyRent.Domain.Enums;
using EasyRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyRent.Infrastructure.Repositories;

/// <summary>EF Core data access for apartments, including search/filtering and pagination.</summary>
public class ApartmentRepository : GenericRepository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Apartment> Items, int TotalCount)> SearchAsync(ApartmentSearchDto search)
    {
        // Read-only query → AsNoTracking for performance.
        var query = Context.Apartments.AsNoTracking().Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(search.City))
            query = query.Where(a => a.City.Contains(search.City));

        if (search.MinPrice.HasValue)
            query = query.Where(a => a.PricePerNight >= search.MinPrice.Value);

        if (search.MaxPrice.HasValue)
            query = query.Where(a => a.PricePerNight <= search.MaxPrice.Value);

        if (search.Bedrooms.HasValue)
            query = query.Where(a => a.Bedrooms >= search.Bedrooms.Value);

        // Availability filter: exclude apartments that already have an Approved/Paid booking
        // overlapping the requested dates. Overlap = (existing.CheckIn < requested.CheckOut)
        // AND (requested.CheckIn < existing.CheckOut).
        if (search.CheckIn.HasValue && search.CheckOut.HasValue)
        {
            var checkIn = search.CheckIn.Value;
            var checkOut = search.CheckOut.Value;
            query = query.Where(a => !a.Bookings.Any(b =>
                (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Paid) &&
                b.CheckIn < checkOut && checkIn < b.CheckOut));
        }

        var totalCount = await query.CountAsync();

        // Defensive paging defaults.
        var page = search.Page < 1 ? 1 : search.Page;
        var pageSize = search.PageSize < 1 ? 10 : search.PageSize;

        var items = await query
            .OrderByDescending(a => a.Id) // newest first
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Apartment>> GetByLandlordAsync(string landlordId) =>
        await Context.Apartments.AsNoTracking()
            .Where(a => a.LandlordId == landlordId)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
}
