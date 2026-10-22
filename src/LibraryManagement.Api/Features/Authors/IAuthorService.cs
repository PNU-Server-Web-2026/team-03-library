namespace LibraryManagement.Api.Features.Authors;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}