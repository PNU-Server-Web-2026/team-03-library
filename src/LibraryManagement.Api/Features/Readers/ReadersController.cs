using LibraryManagement.Api.Features.Readers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Features.Readers;

[ApiController]
[Route("api/readers")]
public sealed class ReadersController : ControllerBase
{
    private readonly IReaderService _readerService;

    public ReadersController(IReaderService readerService)
    {
        _readerService = readerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReaderResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var readers = await _readerService.GetAllAsync(cancellationToken);

        return Ok(readers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReaderResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var reader = await _readerService.GetByIdAsync(
            id,
            cancellationToken);

        if (reader is null)
        {
            return NotFound();
        }

        return Ok(reader);
    }

    [HttpPost]
    public async Task<ActionResult<ReaderResponse>> Create(
        CreateReaderRequest request,
        CancellationToken cancellationToken)
    {
        var reader = await _readerService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = reader.Id },
            reader);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateReaderRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _readerService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _readerService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
