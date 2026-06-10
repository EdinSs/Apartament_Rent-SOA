using Microsoft.AspNetCore.Identity;

namespace EasyRent.Domain.Entities;

/// <summary>
/// The application user. Extends ASP.NET Core Identity's <see cref="IdentityUser"/> so we
/// inherit secure password hashing, email, and account-lockout out of the box. The user's
/// role (Admin / Landlord / Tenant) is managed through Identity roles. Domain-specific
/// fields and navigation properties are added below.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>The user's full display name.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Apartments owned by this user (populated when the user is a Landlord).</summary>
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    /// <summary>Bookings made by this user (populated when the user is a Tenant).</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
