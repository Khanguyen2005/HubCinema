namespace HubCinema.Common.Responses;

/// <summary>Standard envelope for API responses that carry a payload.</summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public T? Data { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>Envelope for responses without a payload (e.g. deletes, errors).</summary>
public class ApiResponse
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse Ok(string? message = null)
        => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, IReadOnlyList<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
