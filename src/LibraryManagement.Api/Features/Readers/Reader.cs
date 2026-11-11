namespace LibraryManagement.Api.Features.Readers;

public sealed class Reader
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public DateOnly RegisteredAt { get; set; }
}
