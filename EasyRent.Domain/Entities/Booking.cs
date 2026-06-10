using EasyRent.Domain.Enums;

namespace EasyRent.Domain.Entities;

/// <summary>
/// A tenant's reservation of an apartment for a date range. This entity holds the
/// booking state-machine status and the server-calculated total price — the heart of
/// the application's complex business logic (availability + workflow + payment).
/// </summary>
public class Booking
{
    public int Id { get; set; }

    // ----- The booked apartment -----
    public int ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    // ----- The tenant who booked (Identity user id is a string) -----
    public string TenantId { get; set; } = string.Empty;
    public ApplicationUser? Tenant { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    /// <summary>Total price computed server-side (nights × apartment PricePerNight).</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Current state in the booking workflow.</summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>The payment for this booking (one-to-one), set once the tenant pays.</summary>
    public Payment? Payment { get; set; }
}
