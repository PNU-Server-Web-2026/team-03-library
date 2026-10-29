using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Features.BookCopies;

/// <summary>
/// CRUD фізичних екземплярів книг.
/// </summary>
[ApiController]
[Route("api/book-copies")]
[Produces("application/json")]
public class BookCopiesController(IBookCopyService service) : ControllerBase
{
    /// <summary>
    /// GET /api/book-copies — усі екземпляри.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BookCopyResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BookCopyResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var copies = await service.GetAllAsync(cancellationToken);
        return Ok(copies);
    }

    /// <summary>
    /// GET /api/book-copies/{id} — екземпляр за Id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<BookCopyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookCopyResponse>> GetById(
        int id, CancellationToken cancellationToken)
    {
        var copy = await service.GetByIdAsync(id, cancellationToken);
        return copy is null ? NotFound() : Ok(copy);
    }

    /// <summary>
    /// POST /api/book-copies — створити екземпляр.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<BookCopyResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<BookCopyResponse>> Create(
        CreateBookCopyRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// PUT /api/book-copies/{id} — оновити екземпляр.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateBookCopyRequest request, CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// DELETE /api/book-copies/{id} — видалити екземпляр.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
