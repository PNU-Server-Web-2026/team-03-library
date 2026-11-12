using LibraryManagement.Api.Features.Books.DTOs;

namespace LibraryManagement.Api.Features.Books;

public interface IBookService
{
    Task<IEnumerable<BookResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<BookResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<BookResponse> CreateAsync(
        CreateBookRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateBookRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
