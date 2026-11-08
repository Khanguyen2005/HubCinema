namespace HubCinema.Common.Exceptions;

/// <summary>Authentication failed or credentials are invalid. Maps to HTTP 401.</summary>
public class UnauthorizedException : AppException
{
    public override int StatusCode => 401;

    public UnauthorizedException(string message = "Unauthorized.")
        : base(message)
    {
    }
}
