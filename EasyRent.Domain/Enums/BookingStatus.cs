namespace EasyRent.Domain.Enums;

/// <summary>
/// The lifecycle states of a <see cref="Entities.Booking"/>.
/// Transitions are enforced in the application/service layer:
/// Pending → Approved/Rejected → Paid → Completed (or Cancelled).
/// </summary>
public enum BookingStatus
{
    /// <summary>Tenant requested the booking; awaiting the landlord's decision.</summary>
    Pending,

    /// <summary>Landlord approved the request; awaiting payment.</summary>
    Approved,

    /// <summary>Landlord rejected the request.</summary>
    Rejected,

    /// <summary>Tenant has paid; the booking is confirmed.</summary>
    Paid,

    /// <summary>The booking was cancelled (by the tenant before payment).</summary>
    Cancelled,

    /// <summary>The stay finished (after checkout).</summary>
    Completed
}
