namespace LibraryManagement.Api.Features.Readers;

public sealed record CreateReaderRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CardNumber);

public sealed record UpdateReaderRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CardNumber);

public sealed record ReaderResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CardNumber,
    DateOnly RegisteredAt)
{
    public static ReaderResponse From(Reader reader) =>
        new(
            reader.Id,
            reader.FirstName,
            reader.LastName,
            reader.Email,
            reader.Phone,
            reader.CardNumber,
            reader.RegisteredAt);
}
