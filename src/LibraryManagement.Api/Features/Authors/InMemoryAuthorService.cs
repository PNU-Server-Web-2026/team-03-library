using System.Collections.Concurrent;

namespace LibraryManagement.Api.Features.Authors;

public class InMemoryAuthorService : IAuthorService
{
    private readonly ConcurrentDictionary<int, Author> _authors = new();
    private int _currentId;

    public Task<IReadOnlyList<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AuthorResponse> result = _authors.Values
            .Select(MapToResponse)
            .OrderBy(a => a.Id)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<AuthorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _authors.TryGetValue(id, out var author);
        return Task.FromResult(author is not null ? MapToResponse(author) : null);
    }

    public Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _currentId);
        var author = new Author
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthYear = request.BirthYear,
            Country = request.Country
        };

        _authors[id] = author;
        return Task.FromResult(MapToResponse(author));
    }

    public Task<bool> UpdateAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        if (!_authors.TryGetValue(id, out var author))
        {
            return Task.FromResult(false);
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.BirthYear = request.BirthYear;
        author.Country = request.Country;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var removed = _authors.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    private static AuthorResponse MapToResponse(Author author) =>
        new(author.Id, author.FirstName, author.LastName, author.BirthYear, author.Country);
}