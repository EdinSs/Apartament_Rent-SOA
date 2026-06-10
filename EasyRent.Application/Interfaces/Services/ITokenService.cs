using EasyRent.Domain.Entities;

namespace EasyRent.Application.Interfaces.Services;

/// <summary>Builds signed JWT access tokens for authenticated users.</summary>
public interface ITokenService
{
    /// <summary>Creates a signed JWT carrying the user's id, email and role.</summary>
    string GenerateToken(ApplicationUser user, string role);
}
