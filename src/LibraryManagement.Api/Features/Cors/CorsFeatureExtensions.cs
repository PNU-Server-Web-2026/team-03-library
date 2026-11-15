using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Api.Features.Cors;

public static class CorsFeatureExtensions
{
    public const string PolicyName = "FrontendCorsPolicy";

    public static IServiceCollection AddCorsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, builder =>
            {
                builder.WithOrigins(origins)
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsFeature(this IApplicationBuilder app)
    {
        return app.UseCors(PolicyName);
    }
}
