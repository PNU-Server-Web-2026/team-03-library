namespace LibraryManagement.Api.Features.Genres;

public static class GenresFeatureExtensions
{
    public static IServiceCollection AddGenresFeature(this IServiceCollection services)
    {
        services.AddSingleton<IGenreService, InMemoryGenreService>();
        return services;
    }
}