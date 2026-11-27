namespace LibraryManagement.Api.Features.BookCopies;

public static class BookCopiesFeatureExtensions
{
    public static IServiceCollection AddBookCopiesFeature(this IServiceCollection services)
    {
        // Singleton: дані живуть у полі сервісу, тож один екземпляр має пережити всі запити.
        // Scoped/Transient створювали б нове порожнє сховище на кожен запит.
        services.AddSingleton<IBookCopyService, InMemoryBookCopyService>();
        return services;
    }
}
