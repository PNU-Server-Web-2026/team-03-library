using System.Collections.Concurrent;

namespace LibraryManagement.Api.Features.Authors;

public class InMemoryAuthorService : IAuthorService
{
    private readonly ConcurrentDictionary<int, Author> _authors = new();
    private int _idCounter = 0;

    public Task<IReadOnlyList<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<AuthorResponse> responses = _authors.Values.Select(a => new AuthorResponse(
            a.Id,
            a.FirstName,
            a.LastName,
            a.BirthYear,
            a.Country
        )).ToList();

        return Task.FromResult(responses);
    }

    public Task<AuthorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_authors.TryGetValue(id, out var author))
        {
            var response = new AuthorResponse(
                author.Id,
                author.FirstName,
                author.LastName,
                author.BirthYear,
                author.Country
            );
            return Task.FromResult<AuthorResponse?>(response);
        }

        return Task.FromResult<AuthorResponse?>(null);
    }

    public Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _idCounter);
        var author = new Author
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthYear = request.BirthYear,
            Country = request.Country
        };

        _authors[id] = author;

        var response = new AuthorResponse(
            author.Id,
            author.FirstName,
            author.LastName,
            author.BirthYear,
            author.Country
        );

        return Task.FromResult(response);
    }

    public Task<bool> UpdateAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_authors.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        bool updated = false;

        _authors.AddOrUpdate(
            id,
            _ => throw new KeyNotFoundException(),
            (key, existingAuthor) =>
            {
                var newAuthor = new Author
                {
                    Id = existingAuthor.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BirthYear = request.BirthYear,
                    Country = request.Country
                };
                updated = true;
                return newAuthor;
            }
        );

        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var removed = _authors.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}