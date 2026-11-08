using Microsoft.AspNetCore.Mvc;

namespace HubCinema.API.Controllers;

/// <summary>
/// Temporary smoke-test controller. Returns hard-coded data so we can confirm
/// the API boots and routing works. Will be replaced by the real Movie module
/// (Controller -> Service -> Repository -> EF) later.
/// </summary>
[ApiController]
[Route("api/[controller]")] // -> GET /api/movies
public class MoviesController : ControllerBase
{
    private static readonly MovieDto[] MockMovies =
    [
        new(1, "Dune: Part Two", "Sci-Fi", 166),
        new(2, "Inside Out 2", "Animation", 96),
        new(3, "The Batman", "Action", 176),
    ];

    [HttpGet]
    public ActionResult<IEnumerable<MovieDto>> GetAll()
    {
        return Ok(MockMovies);
    }
}

/// <summary>Temporary inline DTO for the smoke test.</summary>
public record MovieDto(int Id, string Title, string Genre, int DurationMinutes);
