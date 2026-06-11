using System.Security.Claims;
using EasyRent.API.Controllers;
using EasyRent.Application.DTOs.Apartments;
using EasyRent.Application.DTOs.Common;
using EasyRent.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EasyRent.Tests.Controllers;

/// <summary>
/// Unit tests for ApartmentsController: asserts status codes, returned types, and that the
/// authenticated user's id/role flow correctly from the JWT claims into the service calls.
/// </summary>
public class ApartmentsControllerTests
{
    private readonly Mock<IApartmentService> _service = new();
    private readonly ApartmentsController _sut;

    public ApartmentsControllerTests()
    {
        _sut = new ApartmentsController(_service.Object);

        // Simulate an authenticated Landlord with id "L1".
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "L1"),
            new Claim(ClaimTypes.Role, "Landlord")
        }, "test"));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Search_returns_Ok_with_paged_result()
    {
        _service.Setup(s => s.SearchAsync(It.IsAny<ApartmentSearchDto>()))
                .ReturnsAsync(new PagedResult<ApartmentDto> { TotalCount = 2 });
        var result = await _sut.Search(new ApartmentSearchDto());
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_returns_Ok()
    {
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ApartmentDto { Id = 1 });
        var result = await _sut.GetById(1);
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_passes_landlordId_from_claims_and_returns_Created()
    {
        var dto = new CreateApartmentDto { Title = "A" };
        _service.Setup(s => s.CreateAsync("L1", dto)).ReturnsAsync(new ApartmentDto { Id = 7 });

        var result = await _sut.Create(dto);

        result.Should().BeOfType<CreatedAtActionResult>();
        _service.Verify(s => s.CreateAsync("L1", dto), Times.Once);
    }

    [Fact]
    public async Task Delete_passes_user_and_isAdmin_and_returns_NoContent()
    {
        var result = await _sut.Delete(3);

        result.Should().BeOfType<NoContentResult>();
        // Landlord (not admin) → isAdmin must be false, userId "L1".
        _service.Verify(s => s.DeleteAsync("L1", false, 3), Times.Once);
    }
}
