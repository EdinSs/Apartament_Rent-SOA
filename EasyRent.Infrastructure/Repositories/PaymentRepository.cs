using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Domain.Entities;
using EasyRent.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyRent.Infrastructure.Repositories;

/// <summary>EF Core data access for payments.</summary>
public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    /// <summary>Payments reach a tenant through their booking, so we filter via Booking.TenantId.</summary>
    public async Task<IEnumerable<Payment>> GetByTenantAsync(string tenantId) =>
        await Context.Payments.AsNoTracking()
            .Include(p => p.Booking)
            .Where(p => p.Booking!.TenantId == tenantId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();
}
