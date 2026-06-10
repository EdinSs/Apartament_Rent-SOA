using EasyRent.Application.DTOs.Auth;
using EasyRent.Application.Interfaces.Services;
using EasyRent.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyRent.Application.Services;

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
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new Exception("Bu email zaten kayıtlı.");

        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email    = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

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
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new Exception("Kullanıcı bulunamadı.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            throw new Exception("Şifre hatalı.");

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