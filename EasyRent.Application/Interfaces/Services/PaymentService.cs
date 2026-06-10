using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Payments;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Application.Interfaces.Services;
using EasyRent.Application.Mapping;
using EasyRent.Domain.Entities;
using EasyRent.Domain.Enums;

namespace EasyRent.Application.Services;

/// <summary>
/// Handles payment of an approved booking. Creating the Payment and flipping the booking
/// to Paid happen through the same DbContext in a single SaveChanges — one atomic
/// transaction, so we can never end up with a payment but an unpaid booking (or vice-versa).
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IBookingRepository _bookingRepo;

    public PaymentService(IPaymentRepository paymentRepo, IBookingRepository bookingRepo)
    {
        _paymentRepo = paymentRepo;
        _bookingRepo = bookingRepo;
    }

    public async Task<PaymentDto> PayAsync(string tenantId, CreatePaymentDto dto)
    {
        var booking = await _bookingRepo.GetByIdAsync(dto.BookingId)
                      ?? throw new NotFoundException("Booking not found.");

        if (booking.TenantId != tenantId)
            throw new ForbiddenException("You can only pay for your own bookings.");

        // State-machine guard: payment is allowed only from the Approved state.
        if (booking.Status != BookingStatus.Approved)
            throw new BusinessRuleException("Only an approved booking can be paid.");

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount    = booking.TotalPrice,           // amount taken from the booking, not the client
            Method    = string.IsNullOrWhiteSpace(dto.Method) ? "DirectDebit" : dto.Method,
            Status    = PaymentStatus.Completed,
            PaidAt    = DateTime.UtcNow
        };

        // Both operations share one DbContext → persisted atomically in a single SaveChanges.
        booking.Status = BookingStatus.Paid;
        _bookingRepo.Update(booking);
        await _paymentRepo.AddAsync(payment);
        await _paymentRepo.SaveChangesAsync();

        return payment.ToDto();
    }

    public async Task<IEnumerable<PaymentDto>> GetMineAsync(string tenantId) =>
        (await _paymentRepo.GetByTenantAsync(tenantId)).Select(p => p.ToDto());
}