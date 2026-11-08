namespace HubCinema.Common.Exceptions;

/// <summary>Request conflicts with current state (e.g. duplicate). Maps to HTTP 409.</summary>
public class ConflictException : AppException
{
    public override int StatusCode => 409;

    public ConflictException(string message)
        : base(message)
    {
    }
}
