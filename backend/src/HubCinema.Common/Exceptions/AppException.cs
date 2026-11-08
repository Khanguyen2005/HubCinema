namespace HubCinema.Common.Exceptions;

/// <summary>
/// Base type for expected, domain-level failures. Each subclass maps to an HTTP
/// status code so the exception-handling middleware can translate it uniformly.
/// Unexpected exceptions (not deriving from this) become 500s.
/// </summary>
public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }

    protected AppException(string message)
        : base(message)
    {
    }
}
