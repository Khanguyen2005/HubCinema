namespace HubCinema.Common.Exceptions;

/// <summary>Requested resource does not exist. Maps to HTTP 404.</summary>
public class NotFoundException : AppException
{
    public override int StatusCode => 404;

    public NotFoundException(string message)
        : base(message)
    {
    }
}
