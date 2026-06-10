namespace EasyRent.Domain.Enums;

/// <summary>
/// The result states of a <see cref="Entities.Payment"/>.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Payment created but not yet settled.</summary>
    Pending,

    /// <summary>Payment completed successfully.</summary>
    Completed,

    /// <summary>Payment attempt failed.</summary>
    Failed
}
