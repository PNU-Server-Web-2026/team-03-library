using System.Collections.Concurrent;
using LibraryManagement.Api.Features.Readers;

namespace LibraryManagement.Api.Features.Readers;

public sealed class InMemoryReaderService : IReaderService
{
    private readonly ConcurrentDictionary<int, Reader> _readers = new();

    private int _nextId;

    public Task<IEnumerable<ReaderResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var readers = _readers.Values
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult<IEnumerable<ReaderResponse>>(readers);
    }

    public Task<ReaderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_readers.TryGetValue(id, out var reader))
        {
            return Task.FromResult<ReaderResponse?>(null);
        }

        return Task.FromResult<ReaderResponse?>(
            MapToResponse(reader));
    }

    public Task<ReaderResponse> CreateAsync(
        CreateReaderRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _nextId);

        var reader = new Reader
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            CardNumber = request.CardNumber,
            RegisteredAt = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _readers[id] = reader;

        return Task.FromResult(MapToResponse(reader));
    }

    public Task<bool> UpdateAsync(
        int id,
        UpdateReaderRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_readers.TryGetValue(id, out var existingReader))
        {
            return Task.FromResult(false);
        }

        var updatedReader = new Reader
        {
            Id = existingReader.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            CardNumber = request.CardNumber,
            RegisteredAt = existingReader.RegisteredAt
        };

        var updated = _readers.TryUpdate(
            id,
            updatedReader,
            existingReader);

        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deleted = _readers.TryRemove(id, out _);

        return Task.FromResult(deleted);
    }

    private static ReaderResponse MapToResponse(Reader reader)
    {
        return new ReaderResponse(
            reader.Id,
            reader.FirstName,
            reader.LastName,
            reader.Email,
            reader.Phone,
            reader.CardNumber,
            reader.RegisteredAt);
    }
}
