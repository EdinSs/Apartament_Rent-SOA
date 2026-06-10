using EasyRent.Domain.Enums;

namespace EasyRent.Domain.Entities;

/// <summary>
/// A payment for a booking (one-to-one). Created when a tenant pays an approved booking;
/// creating it flips the booking's status to Paid (handled in the service layer).
/// </summary>
public class Payment
{
    public int Id { get; set; }

    // ----- The booking being paid -----
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Payment method label (mock direct debit for this project).</summary>
    public string Method { get; set; } = "DirectDebit";

    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}
