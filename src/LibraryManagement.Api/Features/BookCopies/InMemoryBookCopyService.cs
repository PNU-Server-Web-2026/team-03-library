using System.Collections.Concurrent;

namespace LibraryManagement.Api.Features.BookCopies;

/// <summary>
/// Тимчасове сховище в пам'яті до появи EF Core. Реєструється як Singleton,
/// тому має бути потокобезпечним: його одночасно використовують усі запити.
/// </summary>
public sealed class InMemoryBookCopyService : IBookCopyService
{
    private readonly ConcurrentDictionary<int, BookCopy> _copies = new();
    private int _lastId;

    public Task<IReadOnlyList<BookCopyResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<BookCopyResponse> result = _copies.Values
            .OrderBy(c => c.Id)
            .Select(BookCopyResponse.From)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<BookCopyResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = _copies.TryGetValue(id, out var copy)
            ? BookCopyResponse.From(copy)
            : null;
        return Task.FromResult(result);
    }

    public Task<BookCopyResponse> CreateAsync(
        CreateBookCopyRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _lastId);
        var copy = new BookCopy(id, request.InventoryNumber, request.Condition, request.Location);
        _copies[id] = copy;
        return Task.FromResult(BookCopyResponse.From(copy));
    }

    public Task<bool> UpdateAsync(
        int id, UpdateBookCopyRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // TryUpdate замінює запис, лише якщо його ніхто не змінив і не видалив паралельно.
        while (_copies.TryGetValue(id, out var current))
        {
            var updated = current with
            {
                InventoryNumber = request.InventoryNumber,
                Condition = request.Condition,
                Location = request.Location,
            };
            if (_copies.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_copies.TryRemove(id, out _));
    }
}
