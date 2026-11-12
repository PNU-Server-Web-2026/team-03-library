using System.Net;
using System.Net.Http.Json;
using LibraryManagement.Api.Features.Genres;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LibraryManagement.Api.Tests.Features.Genres;

public class GenresApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GenresApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndArray()
    {
        var response = await _client.GetAsync("/api/genres");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genres = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        Assert.NotNull(genres);
    }

    [Fact]
    public async Task CrudLifecycle_WorksCorrectly()
    {
        var createRequest = new CreateGenreRequest("Fantasy");
        var postResponse = await _client.PostAsJsonAsync("/api/genres", createRequest);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        var created = await postResponse.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(created);
        Assert.Equal("Fantasy", created!.Name);

        var getResponse = await _client.GetAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateRequest = new UpdateGenreRequest("Dark Fantasy");
        var putResponse = await _client.PutAsJsonAsync($"/api/genres/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await _client.DeleteAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var notFoundResponse = await _client.GetAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
    }
}