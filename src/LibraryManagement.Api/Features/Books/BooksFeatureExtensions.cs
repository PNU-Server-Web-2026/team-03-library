namespace LibraryManagement.Api.Features.Books;

public static class BooksFeatureExtensions
{
    public static IServiceCollection AddBooksFeature(
        this IServiceCollection services)
    {
        services.AddSingleton<IBookService, InMemoryBookService>();

        return services;
    }
}
