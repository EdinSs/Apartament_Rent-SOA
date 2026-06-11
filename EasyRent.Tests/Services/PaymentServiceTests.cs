using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Payments;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Application.Services;
using EasyRent.Domain.Entities;
using EasyRent.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace EasyRent.Tests.Services;

/// <summary>Unit tests for PaymentService: pay-only-approved + atomic Booking→Paid.</summary>
public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepo = new();
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly PaymentService _sut;

    public PaymentServiceTests() =>
        _sut = new PaymentService(_paymentRepo.Object, _bookingRepo.Object);

    [Fact]
    public async Task PayAsync_approved_creates_payment_and_marks_booking_Paid()
    {
        var booking = new Booking { Id = 5, TenantId = "T1", Status = BookingStatus.Approved, TotalPrice = 300 };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);

        var result = await _sut.PayAsync("T1", new CreatePaymentDto { BookingId = 5 });

        result.Amount.Should().Be(300);
        result.Status.Should().Be("Completed");
        booking.Status.Should().Be(BookingStatus.Paid);
        _paymentRepo.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task PayAsync_unapproved_booking_is_rejected()
    {
        var booking = new Booking { Id = 5, TenantId = "T1", Status = BookingStatus.Pending, TotalPrice = 300 };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var act = () => _sut.PayAsync("T1", new CreatePaymentDto { BookingId = 5 });
        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*approved*");
    }

    [Fact]
    public async Task PayAsync_someone_elses_booking_is_forbidden()
    {
        var booking = new Booking { Id = 5, TenantId = "T1", Status = BookingStatus.Approved, TotalPrice = 300 };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var act = () => _sut.PayAsync("OTHER", new CreatePaymentDto { BookingId = 5 });
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
