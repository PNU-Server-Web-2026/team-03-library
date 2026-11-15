namespace LibraryManagement.Api.Features.Genres;

public interface IGenreService
{
    Task<IReadOnlyList<GenreResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GenreResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GenreResponse> CreateAsync(CreateGenreRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateGenreRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}