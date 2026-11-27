using System.Text.Json.Serialization;

namespace LibraryManagement.Api.Features.BookCopies;

/// <summary>
/// Фізичний стан екземпляра. У JSON передається рядком ("Good"), а не числом.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<BookCopyCondition>))]
public enum BookCopyCondition
{
    New,
    Good,
    Worn,
    Damaged,
}
