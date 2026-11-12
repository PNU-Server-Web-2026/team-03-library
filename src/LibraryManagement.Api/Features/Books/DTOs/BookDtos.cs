using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Features.Books.DTOs;

public sealed record CreateBookRequest(
    [Required, StringLength(300)] string Title,
    [RegularExpression(@"^(?:\d{13}|\d{3}-\d-\d{2}-\d{6}-\d)$")] string Isbn,
    int PublishYear,
    [Range(1, 10000)] int Pages,
    [RegularExpression(@"^[a-z]{2}$")] string Language);

public sealed record UpdateBookRequest(
    [Required, StringLength(300)] string Title,
    [RegularExpression(@"^(?:\d{13}|\d{3}-\d-\d{2}-\d{6}-\d)$")] string Isbn,
    int PublishYear,
    [Range(1, 10000)] int Pages,
    [RegularExpression(@"^[a-z]{2}$")] string Language);

public sealed record BookResponse(
    int Id,
    string Title,
    string Isbn,
    int PublishYear,
    int Pages,
    string Language);
