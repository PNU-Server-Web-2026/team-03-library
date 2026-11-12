namespace LibraryManagement.Api.Features.Books;

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public int Pages { get; set; }
    public string Language { get; set; } = string.Empty;
}
