namespace EasyRent.Domain.Entities;

/// <summary>
/// An apartment listing published by a Landlord and bookable by Tenants.
/// This is the root entity for the CRUD feature and the parent of bookings.
/// </summary>
public class Apartment
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    /// <summary>Nightly price; the server derives a booking's total from this value.</summary>
    public decimal PricePerNight { get; set; }

    public int Bedrooms { get; set; }

    /// <summary>When false, the listing is hidden from search (soft-disable).</summary>
    public bool IsActive { get; set; } = true;

    public string? PhotoUrl { get; set; }

    // ----- Relationships -----

    /// <summary>FK to the owning Landlord (Identity user ids are strings).</summary>
    public string LandlordId { get; set; } = string.Empty;
    public ApplicationUser? Landlord { get; set; }

    /// <summary>Bookings made against this apartment.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
