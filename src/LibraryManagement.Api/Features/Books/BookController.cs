using LibraryManagement.Api.Features.Books.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Features.Books;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var books = await _bookService.GetAllAsync(cancellationToken);

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(
        CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var book = await _bookService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = book.Id },
            book);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _bookService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _bookService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
