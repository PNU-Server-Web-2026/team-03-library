using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Features.Genres;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GenreResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var genres = await _genreService.GetAllAsync(cancellationToken);
        return Ok(genres);
    }

    [HttpGet("{id:int}", Name = "GetGenreById")]
    public async Task<ActionResult<GenreResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var genre = await _genreService.GetByIdAsync(id, cancellationToken);
        if (genre is null)
        {
            return NotFound();
        }

        return Ok(genre);
    }

    [HttpPost]
    public async Task<ActionResult<GenreResponse>> Create([FromBody] CreateGenreRequest request, CancellationToken cancellationToken)
    {
        var createdGenre = await _genreService.CreateAsync(request, cancellationToken);
        return CreatedAtRoute("GetGenreById", new { id = createdGenre.Id }, createdGenre);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreRequest request, CancellationToken cancellationToken)
    {
        var updated = await _genreService.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _genreService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}