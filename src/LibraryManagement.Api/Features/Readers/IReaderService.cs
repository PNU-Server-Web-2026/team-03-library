using LibraryManagement.Api.Features.Readers;

namespace LibraryManagement.Api.Features.Readers;

public interface IReaderService
{
    Task<IEnumerable<ReaderResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ReaderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<ReaderResponse> CreateAsync(
        CreateReaderRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateReaderRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}
