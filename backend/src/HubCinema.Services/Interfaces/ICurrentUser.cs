namespace HubCinema.Services.Interfaces;

/// <summary>
/// The authenticated user of the current request. Implemented in the API layer, which
/// reads the values from the validated JWT claims.
/// </summary>
public interface ICurrentUser
{
    int? UserId { get; }

    bool IsAuthenticated { get; }
}
