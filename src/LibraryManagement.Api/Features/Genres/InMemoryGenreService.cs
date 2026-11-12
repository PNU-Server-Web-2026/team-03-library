using System.Collections.Concurrent;

namespace LibraryManagement.Api.Features.Genres;

public class InMemoryGenreService : IGenreService
{
    private readonly ConcurrentDictionary<int, Genre> _genres = new();
    private int _idCounter = 0;

    public Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<GenreResponse> responses = _genres.Values.Select(g => new GenreResponse(
            g.Id,
            g.Name
        )).ToList();

        return Task.FromResult(responses);
    }

    public Task<GenreResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_genres.TryGetValue(id, out var genre))
        {
            var response = new GenreResponse(genre.Id, genre.Name);
            return Task.FromResult<GenreResponse?>(response);
        }

        return Task.FromResult<GenreResponse?>(null);
    }

    public Task<GenreResponse> CreateAsync(CreateGenreRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _idCounter);
        var genre = new Genre
        {
            Id = id,
            Name = request.Name
        };

        _genres[id] = genre;

        var response = new GenreResponse(genre.Id, genre.Name);
        return Task.FromResult(response);
    }

    public Task<bool> UpdateAsync(int id, UpdateGenreRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_genres.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        bool updated = false;

        _genres.AddOrUpdate(
            id,
            _ => throw new KeyNotFoundException(),
            (key, existingGenre) =>
            {
                var newGenre = new Genre
                {
                    Id = existingGenre.Id,
                    Name = request.Name
                };
                updated = true;
                return newGenre;
            }
        );

        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var removed = _genres.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}