using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Apartments;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Application.Services;
using EasyRent.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace EasyRent.Tests.Services;

/// <summary>Unit tests for ApartmentService: CRUD + the ownership rule.</summary>
public class ApartmentServiceTests
{
    private readonly Mock<IApartmentRepository> _repo = new();
    private readonly ApartmentService _sut;

    public ApartmentServiceTests() => _sut = new ApartmentService(_repo.Object);

    [Fact]
    public async Task GetByIdAsync_throws_when_missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Apartment?)null);
        var act = () => _sut.GetByIdAsync(99);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_sets_owner_and_saves()
    {
        var dto = new CreateApartmentDto { Title = "A", City = "Skopje", Address = "x", PricePerNight = 50, Bedrooms = 2 };
        var result = await _sut.CreateAsync("L1", dto);

        result.LandlordId.Should().Be("L1");
        result.Title.Should().Be("A");
        _repo.Verify(r => r.AddAsync(It.IsAny<Apartment>()), Times.Once);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_by_non_owner_is_forbidden()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Apartment { Id = 1, LandlordId = "OWNER" });
        var dto = new UpdateApartmentDto { Title = "X", City = "Y", Address = "z", PricePerNight = 10, Bedrooms = 1 };
        var act = () => _sut.UpdateAsync("INTRUDER", 1, dto);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteAsync_admin_can_delete_any_apartment()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Apartment { Id = 1, LandlordId = "OWNER" });
        await _sut.DeleteAsync("SOME_ADMIN", isAdmin: true, 1);
        _repo.Verify(r => r.Delete(It.IsAny<Apartment>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_non_owner_non_admin_is_forbidden()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Apartment { Id = 1, LandlordId = "OWNER" });
        var act = () => _sut.DeleteAsync("INTRUDER", isAdmin: false, 1);
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
