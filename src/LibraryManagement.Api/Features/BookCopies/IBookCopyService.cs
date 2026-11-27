namespace LibraryManagement.Api.Features.BookCopies;

public interface IBookCopyService
{
    Task<IReadOnlyList<BookCopyResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<BookCopyResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<BookCopyResponse> CreateAsync(
        CreateBookCopyRequest request, CancellationToken cancellationToken);

    /// <returns><c>false</c>, якщо екземпляра з таким Id немає.</returns>
    Task<bool> UpdateAsync(
        int id, UpdateBookCopyRequest request, CancellationToken cancellationToken);

    /// <returns><c>false</c>, якщо екземпляра з таким Id немає.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
