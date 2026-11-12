using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Features.Authors;

public record CreateAuthorRequest(
    [Required] [MaxLength(100)] string FirstName,
    [Required] [MaxLength(100)] string LastName,
    int? BirthYear,
    [MaxLength(100)] string? Country
);

public record UpdateAuthorRequest(
    [Required] [MaxLength(100)] string FirstName,
    [Required] [MaxLength(100)] string LastName,
    int? BirthYear,
    [MaxLength(100)] string? Country
);

public record AuthorResponse(
    int Id,
    string FirstName,
    string LastName,
    int? BirthYear,
    string? Country
);