namespace EasyRent.Domain.Entities;

/// <summary>
/// A tenant's rating and comment for an apartment. Business rule (enforced in the service
/// layer): only a tenant who has a completed booking for the apartment may leave a review.
/// </summary>
public class Review
{
    public int Id { get; set; }

    // ----- The reviewed apartment -----
    public int ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    // ----- The tenant who wrote the review -----
    public string TenantId { get; set; } = string.Empty;
    public ApplicationUser? Tenant { get; set; }

    /// <summary>Rating from 1 to 5 (range validated in the application layer).</summary>
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
