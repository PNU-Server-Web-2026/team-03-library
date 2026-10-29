using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace LibraryManagement.Api.Tests.Features.OpenApi;

[Collection(ApiCollection.Name)]
public class OpenApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task OpenApi_ShouldContainMetadata_InDevelopment()
    {
        await using var developmentFactory = Factory.WithWebHostBuilder(
            builder => builder.UseEnvironment("Development"));
        using var client = developmentFactory.CreateClient();

        var responseScalar = await client.GetAsync("/scalar");
        Assert.Equal(HttpStatusCode.OK, responseScalar.StatusCode);

        var responseJson = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, responseJson.StatusCode);

        await using var stream = await responseJson.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var info = document.RootElement.GetProperty("info");

        Assert.Equal("Library Management API", info.GetProperty("title").GetString());
        Assert.Equal("v1", info.GetProperty("version").GetString());
        Assert.Contains(
            "API для керування бібліотекою та книгами.",
            info.GetProperty("description").GetString(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task OpenApi_ShouldContainPingSummary_InDevelopment()
    {
        await using var developmentFactory = Factory.WithWebHostBuilder(
            builder => builder.UseEnvironment("Development"));
        using var client = developmentFactory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var summary = document.RootElement
            .GetProperty("paths")
            .GetProperty("/api/ping")
            .GetProperty("get")
            .GetProperty("summary")
            .GetString();
        Assert.Equal("GET /api/ping — повертає \"pong\" і поточний час UTC.", summary);
    }

    [Fact]
    public async Task Documentation_ShouldReturnNotFound_InProduction()
    {
        await using var productionFactory = Factory.WithWebHostBuilder(
            builder => builder.UseEnvironment("Production"));
        using var client = productionFactory.CreateClient();

        var responseScalar = await client.GetAsync("/scalar");
        var responseJson = await client.GetAsync("/openapi/v1.json");
      
        Assert.Equal(HttpStatusCode.NotFound, responseScalar.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responseJson.StatusCode);
    }
}
