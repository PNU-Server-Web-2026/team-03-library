namespace LibraryManagement.Api.Features.BookCopies;

public sealed record CreateBookCopyRequest(
    string InventoryNumber,
    BookCopyCondition Condition,
    string Location);

public sealed record UpdateBookCopyRequest(
    string InventoryNumber,
    BookCopyCondition Condition,
    string Location);

public sealed record BookCopyResponse(
    int Id,
    string InventoryNumber,
    BookCopyCondition Condition,
    string Location)
{
    public static BookCopyResponse From(BookCopy copy) =>
        new(copy.Id, copy.InventoryNumber, copy.Condition, copy.Location);
}
