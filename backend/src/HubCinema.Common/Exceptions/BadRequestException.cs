namespace HubCinema.Common.Exceptions;

/// <summary>Invalid input / failed business validation. Maps to HTTP 400.</summary>
public class BadRequestException : AppException
{
    public override int StatusCode => 400;

    public IReadOnlyList<string> Errors { get; }

    public BadRequestException(string message, IReadOnlyList<string>? errors = null)
        : base(message)
        => Errors = errors ?? [];
}
