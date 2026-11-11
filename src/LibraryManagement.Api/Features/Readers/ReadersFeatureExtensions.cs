namespace LibraryManagement.Api.Features.Readers;

public static class ReadersFeatureExtensions
{
    public static IServiceCollection AddReadersFeature(
        this IServiceCollection services)
    {
        services.AddSingleton<IReaderService, InMemoryReaderService>();

        return services;
    }
}
