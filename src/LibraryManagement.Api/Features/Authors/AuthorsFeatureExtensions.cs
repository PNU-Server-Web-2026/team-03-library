namespace LibraryManagement.Api.Features.Authors;

public static class AuthorsFeatureExtensions
{
    public static IServiceCollection AddAuthorsFeature(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorService, InMemoryAuthorService>();
        return services;
    }
}