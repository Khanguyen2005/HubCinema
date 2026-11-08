using System.Security.Claims;
using HubCinema.Services.Interfaces;

namespace HubCinema.API.Security;

/// <summary>Reads the authenticated user's identity from the current request's JWT claims.</summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId
    {
        get
        {
            // JwtBearer maps the "sub" claim to NameIdentifier by default.
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
