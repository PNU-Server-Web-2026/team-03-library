
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Api.Common;

public static class LibraryOptionsExtensions
{
    public static IServiceCollection AddLibraryOptions(
        this IServiceCollection services)
    {
        services
            .AddOptions<LibraryOptions>()
            .BindConfiguration(LibraryOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
