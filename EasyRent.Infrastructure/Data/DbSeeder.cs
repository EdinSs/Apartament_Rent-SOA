using EasyRent.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRent.Infrastructure.Data;

/// <summary>
/// Seeds the three application roles (Admin / Landlord / Tenant) and a default Admin account.
/// Idempotent — safe to run on every startup; it only creates what is missing.
/// Invoked from Program.cs AFTER Identity is registered (Phase 6).
/// </summary>
public static class DbSeeder
{
    public const string AdminEmail = "admin@easyrent.com";
    private const string AdminPassword = "Admin_123!";

    public static readonly string[] Roles = { "Admin", "Landlord", "Tenant" };

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1) Ensure the roles exist.
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2) Ensure a default Admin user exists (so the system is usable on a fresh DB).
        if (await userManager.FindByEmailAsync(AdminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                FullName = "System Administrator",
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
