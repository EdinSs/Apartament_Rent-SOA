using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Bookings;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Application.Services;
using EasyRent.Domain.Entities;
using EasyRent.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace EasyRent.Tests.Services;

/// <summary>
/// Unit tests for the booking engine — the application's most complex logic.
/// Repositories are mocked with Moq so we test the rules in isolation.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IApartmentRepository> _apartmentRepo = new();
    private readonly BookingService _sut;

    public BookingServiceTests() =>
        _sut = new BookingService(_bookingRepo.Object, _apartmentRepo.Object);

    private static Apartment Apt(string landlord = "L1", decimal price = 100) =>
        new() { Id = 1, Title = "A", LandlordId = landlord, PricePerNight = price };

    private static CreateBookingDto Dates(int d1, int d2) =>
        new() { ApartmentId = 1, CheckIn = new DateTime(2026, 7, d1), CheckOut = new DateTime(2026, 7, d2) };

    [Fact]
    public async Task CreateAsync_computes_price_sets_Pending_and_saves()
    {
        _apartmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Apt(price: 100));
        _bookingRepo.Setup(r => r.HasOverlapAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(false);

        var result = await _sut.CreateAsync("T1", Dates(1, 4)); // 3 nights × 100

        result.TotalPrice.Should().Be(300);
        result.Status.Should().Be("Pending");
        result.ApartmentTitle.Should().Be("A");
        _bookingRepo.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
        _bookingRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_throws_when_apartment_missing()
    {
        _apartmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Apartment?)null);
        var act = () => _sut.CreateAsync("T1", Dates(1, 4));
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_rejects_bad_date_range()
    {
        _apartmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Apt());
        var act = () => _sut.CreateAsync("T1", Dates(5, 5));
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task CreateAsync_rejects_overlapping_dates()
    {
        _apartmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Apt());
        _bookingRepo.Setup(r => r.HasOverlapAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(true);
        var act = () => _sut.CreateAsync("T1", Dates(1, 4));
        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*not available*");
    }

    [Fact]
    public async Task ApproveAsync_by_owning_landlord_sets_Approved()
    {
        var booking = new Booking { Id = 5, Status = BookingStatus.Pending, Apartment = Apt("L1") };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var result = await _sut.ApproveAsync("L1", 5);
        result.Status.Should().Be("Approved");
    }

    [Fact]
    public async Task ApproveAsync_by_wrong_landlord_is_forbidden()
    {
        var booking = new Booking { Id = 5, Status = BookingStatus.Pending, Apartment = Apt("L1") };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var act = () => _sut.ApproveAsync("OTHER", 5);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task ApproveAsync_when_not_pending_is_rejected()
    {
        var booking = new Booking { Id = 5, Status = BookingStatus.Approved, Apartment = Apt("L1") };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var act = () => _sut.ApproveAsync("L1", 5);
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task CancelAsync_by_owner_sets_Cancelled()
    {
        var booking = new Booking { Id = 5, Status = BookingStatus.Pending, TenantId = "T1", Apartment = Apt() };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var result = await _sut.CancelAsync("T1", 5);
        result.Status.Should().Be("Cancelled");
    }

    [Fact]
    public async Task CancelAsync_paid_booking_is_rejected()
    {
        var booking = new Booking { Id = 5, Status = BookingStatus.Paid, TenantId = "T1", Apartment = Apt() };
        _bookingRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(booking);
        var act = () => _sut.CancelAsync("T1", 5);
        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("*paid*");
    }
}
