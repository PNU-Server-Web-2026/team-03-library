using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Features.Genres;

public record CreateGenreRequest(
    [Required] [StringLength(100, MinimumLength = 1)] string Name
);

public record UpdateGenreRequest(
    [Required] [StringLength(100, MinimumLength = 1)] string Name
);

public record GenreResponse(
    int Id,
    string Name
);