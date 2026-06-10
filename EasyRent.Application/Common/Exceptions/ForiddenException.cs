namespace EasyRent.Application.Common.Exceptions;

/// <summary>
/// Thrown when an authenticated user tries to act on a resource they don't own
/// (e.g. editing another landlord's apartment, paying someone else's booking).
/// The API's exception middleware maps this to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}