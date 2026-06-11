using EasyRent.Application.DTOs.Apartments;
using EasyRent.Domain.Entities;
using EasyRent.Infrastructure.Data;
using EasyRent.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EasyRent.Tests.Repositories;

/// <summary>
/// Repository tests against the EF Core InMemory provider — verifies the search filters,
/// pagination, and landlord query actually return the correct rows.
/// </summary>
public class ApartmentRepositoryTests
{
    private static AppDbContext Ctx() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"er_{Guid.NewGuid()}").Options);

    private static Apartment Apt(int id, string city, decimal price) => new()
    {
        Id = id, Title = $"A{id}", Description = "d", Address = "x",
        City = city, PricePerNight = price, Bedrooms = 2, IsActive = true, LandlordId = "L1"
    };

    [Fact]
    public async Task SearchAsync_filters_by_city_and_paginates()
    {
        using var ctx = Ctx();
        for (int i = 1; i <= 15; i++)
            ctx.Apartments.Add(Apt(i, i <= 12 ? "Skopje" : "Tetovo", 50 + i));
        await ctx.SaveChangesAsync();
        var repo = new ApartmentRepository(ctx);

        var (items, total) = await repo.SearchAsync(
            new ApartmentSearchDto { City = "Skopje", Page = 1, PageSize = 10 });

        total.Should().Be(12);
        items.Should().HaveCount(10).And.OnlyContain(a => a.City == "Skopje");
    }

    [Fact]
    public async Task SearchAsync_filters_by_price_range()
    {
        using var ctx = Ctx();
        ctx.Apartments.Add(Apt(1, "Skopje", 40));
        ctx.Apartments.Add(Apt(2, "Skopje", 60));
        ctx.Apartments.Add(Apt(3, "Skopje", 100));
        await ctx.SaveChangesAsync();
        var repo = new ApartmentRepository(ctx);

        var (items, _) = await repo.SearchAsync(new ApartmentSearchDto { MinPrice = 50, MaxPrice = 90 });
        items.Should().ContainSingle(a => a.Id == 2);
    }

    [Fact]
    public async Task SearchAsync_excludes_inactive_apartments()
    {
        using var ctx = Ctx();
        ctx.Apartments.Add(Apt(1, "Skopje", 50));
        var inactive = Apt(2, "Skopje", 50); inactive.IsActive = false;
        ctx.Apartments.Add(inactive);
        await ctx.SaveChangesAsync();
        var repo = new ApartmentRepository(ctx);

        var (items, total) = await repo.SearchAsync(new ApartmentSearchDto());
        total.Should().Be(1);
        items.Single().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByLandlordAsync_returns_only_that_landlords_apartments()
    {
        using var ctx = Ctx();
        ctx.Apartments.Add(Apt(1, "Skopje", 50));
        var other = Apt(2, "Skopje", 50); other.LandlordId = "L2";
        ctx.Apartments.Add(other);
        await ctx.SaveChangesAsync();
        var repo = new ApartmentRepository(ctx);

        var mine = await repo.GetByLandlordAsync("L1");
        mine.Should().ContainSingle(a => a.Id == 1);
    }
}
