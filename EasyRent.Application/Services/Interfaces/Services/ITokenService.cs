using EasyRent.Domain.Entities;

namespace EasyRent.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, string role);
}