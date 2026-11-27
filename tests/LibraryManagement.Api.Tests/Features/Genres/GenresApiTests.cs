using System.Net;
using System.Net.Http.Json;
using LibraryManagement.Api.Features.Genres;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LibraryManagement.Api.Tests.Features.Genres;

public class GenresApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkAndArray()
    {
        var response = await _client.GetAsync("/api/genres");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genres = await response.Content.ReadFromJsonAsync<List<GenreResponse>>();
        Assert.NotNull(genres);
    }

    [Fact]
    public async Task GetById_WhenGenreNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/genres/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesGenre_ReturnsCreatedWithLocationHeaderAndBody()
    {
        var request = new CreateGenreRequest("Fantasy");

        var response = await _client.PostAsJsonAsync("/api/genres", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Fantasy", created.Name);

        // Перевірка GET за Location та збереженого Name
        var getResponse = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetchedByLocation = await getResponse.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(fetchedByLocation);
        Assert.Equal("Fantasy", fetchedByLocation.Name);
    }

    [Fact]
    public async Task Put_WhenGenreExists_UpdatesAndReturnsNoContent()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/genres", new CreateGenreRequest("Sci-Fi"));
        var created = await postResponse.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(created);

        var updateRequest = new UpdateGenreRequest("Science Fiction");
        var putResponse = await _client.PutAsJsonAsync($"/api/genres/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // Перевірка збереженого Name після PUT
        var updatedGenre = await _client.GetFromJsonAsync<GenreResponse>($"/api/genres/{created.Id}");
        Assert.NotNull(updatedGenre);
        Assert.Equal("Science Fiction", updatedGenre.Name);
    }

    [Fact]
    public async Task Put_WhenGenreNotFound_ReturnsNotFound()
    {
        var updateRequest = new UpdateGenreRequest("Unknown");

        var response = await _client.PutAsJsonAsync("/api/genres/99999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenGenreExists_ReturnsNoContentAndDeletes()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/genres", new CreateGenreRequest("Horror"));
        var created = await postResponse.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/genres/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenGenreNotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/genres/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutAndDelete_ConcurrentRequests_DoesNotReturnInternalServerError()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/genres", new CreateGenreRequest("Thriller"));
        var created = await postResponse.Content.ReadFromJsonAsync<GenreResponse>();
        Assert.NotNull(created);

        var updateRequest = new UpdateGenreRequest("Psychological Thriller");

        var putTask = _client.PutAsJsonAsync($"/api/genres/{created.Id}", updateRequest);
        var deleteTask = _client.DeleteAsync($"/api/genres/{created.Id}");

        var responses = await Task.WhenAll(putTask, deleteTask);

        foreach (var response in responses)
        {
            Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.True(
                response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound,
                $"Unexpected status code: {response.StatusCode}"
            );
        }
    }
}