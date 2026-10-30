using System.Net;
using System.Net.Http.Json;
using LibraryManagement.Api.Features.Authors;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LibraryManagement.Api.Tests.Features.Authors;

public class AuthorsApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkAndArray()
    {
        var response = await _client.GetAsync("/api/authors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var authors = await response.Content.ReadFromJsonAsync<List<AuthorResponse>>();
        Assert.NotNull(authors);
    }

    [Fact]
    public async Task GetById_WhenAuthorNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/authors/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesAuthor_ReturnsCreatedWithLocationHeaderAndBody()
    {
        var request = new CreateAuthorRequest("Taras", "Shevchenko", 1814, "Ukraine");

        var response = await _client.PostAsJsonAsync("/api/authors", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<AuthorResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Taras", created.FirstName);
        Assert.Equal("Shevchenko", created.LastName);

        var getResponse = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Put_WhenAuthorExists_UpdatesAndReturnsNoContent()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/authors", 
            new CreateAuthorRequest("Ivan", "Franko", 1856, "Ukraine"));
        var created = await postResponse.Content.ReadFromJsonAsync<AuthorResponse>();
        Assert.NotNull(created);

        var updateRequest = new UpdateAuthorRequest("Ivan", "Franko", 1856, "Austro-Hungarian Empire");
        var putResponse = await _client.PutAsJsonAsync($"/api/authors/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var updatedAuthor = await _client.GetFromJsonAsync<AuthorResponse>($"/api/authors/{created.Id}");
        Assert.NotNull(updatedAuthor);
        Assert.Equal("Austro-Hungarian Empire", updatedAuthor.Country);
    }

    [Fact]
    public async Task Put_WhenAuthorNotFound_ReturnsNotFound()
    {
        var updateRequest = new UpdateAuthorRequest("Unknown", "User", null, null);

        var response = await _client.PutAsJsonAsync("/api/authors/99999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenAuthorExists_ReturnsNoContentAndDeletes()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/authors", 
            new CreateAuthorRequest("Lesya", "Ukrainka", 1871, "Ukraine"));
        var created = await postResponse.Content.ReadFromJsonAsync<AuthorResponse>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/authors/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/authors/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenAuthorNotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/authors/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}