namespace LibraryManagement.Api.Features.BookCopies;

/// <summary>
/// Внутрішня модель екземпляра книги. Назовні не віддається: контролер працює лише з DTO.
/// </summary>
public sealed record BookCopy(
    int Id,
    string InventoryNumber,
    BookCopyCondition Condition,
    string Location);
