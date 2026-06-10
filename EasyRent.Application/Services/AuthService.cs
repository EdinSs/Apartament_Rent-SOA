using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Auth;
using EasyRent.Application.Interfaces.Services;
using EasyRent.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyRent.Application.Services;

/// <summary>Handles user registration and login, returning a JWT on success.</summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            throw new BusinessRuleException("This email is already registered.");

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email    = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);

        var token = _tokenService.GenerateToken(user, dto.Role);

        return new AuthResponseDto
        {
            Token     = token,
            FullName  = user.FullName,
            Role      = dto.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        // Same generic message whether the email or the password is wrong → avoids
        // leaking which accounts exist (user-enumeration protection).
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new BusinessRuleException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? "Tenant";

        var token = _tokenService.GenerateToken(user, role);

        return new AuthResponseDto
        {
            Token     = token,
            FullName  = user.FullName,
            Role      = role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}
