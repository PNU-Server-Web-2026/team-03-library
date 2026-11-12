using System.Collections.Concurrent;
using LibraryManagement.Api.Features.Books.DTOs;

namespace LibraryManagement.Api.Features.Books;

public sealed class InMemoryBookService : IBookService
{
    private readonly ConcurrentDictionary<int, Book> _books = new();
    private int _nextId;

    public Task<IEnumerable<BookResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var books = _books.Values
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult<IEnumerable<BookResponse>>(books);
    }

    public Task<BookResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_books.TryGetValue(id, out var book))
            return Task.FromResult<BookResponse?>(null);

        return Task.FromResult<BookResponse?>(MapToResponse(book));
    }

    public Task<BookResponse> CreateAsync(
        CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _nextId);

        var book = new Book
        {
            Id = id,
            Title = request.Title,
            Isbn = request.Isbn,
            PublishYear = request.PublishYear,
            Pages = request.Pages,
            Language = request.Language
        };

        _books[id] = book;

        return Task.FromResult(MapToResponse(book));
    }

    public Task<bool> UpdateAsync(
        int id,
        UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_books.TryGetValue(id, out var existingBook))
            return Task.FromResult(false);

        var updatedBook = new Book
        {
            Id = id,
            Title = request.Title,
            Isbn = request.Isbn,
            PublishYear = request.PublishYear,
            Pages = request.Pages,
            Language = request.Language
        };

        var updated = _books.TryUpdate(id, updatedBook, existingBook);

        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deleted = _books.TryRemove(id, out _);

        return Task.FromResult(deleted);
    }

    private static BookResponse MapToResponse(Book book) =>
        new(
            book.Id,
            book.Title,
            book.Isbn,
            book.PublishYear,
            book.Pages,
            book.Language);
}
