using System.Net;
using System.Net.Http.Json;
using LibraryManagement.Api.Features.Readers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LibraryManagement.Api.Tests.Features.Readers;

public class ReadersApiTests
{
    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>();
    }

    private static CreateReaderRequest CreateRequest()
    {
        return new CreateReaderRequest(
            "Ivan",
            "Petrenko",
            "ivan@example.com",
            "+380501234567",
            "1234567890");
    }

    private static UpdateReaderRequest UpdateRequest()
    {
        return new UpdateReaderRequest(
            "Petro",
            "Ivanenko",
            "petro@example.com",
            "+380671234567",
            "0987654321");
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/readers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var readers = await response.Content
            .ReadFromJsonAsync<List<ReaderResponse>>();

        Assert.NotNull(readers);
    }

    [Fact]
    public async Task GetById_WhenReaderExists_ReturnsOk()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/readers",
            CreateRequest());

        var createdReader = await createResponse.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(createdReader);

        var response = await client.GetAsync(
            $"/api/readers/{createdReader.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reader = await response.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(reader);
        Assert.Equal(createdReader.Id, reader.Id);
        Assert.Equal("Ivan", reader.FirstName);
    }

    [Fact]
    public async Task GetById_WhenReaderDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/readers/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/readers",
            CreateRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.NotNull(response.Headers.Location);

        var reader = await response.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(reader);
        Assert.True(reader.Id > 0);

        Assert.EndsWith(
            $"/api/readers/{reader.Id}",
            response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Update_WhenReaderExists_ReturnsNoContent()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/readers",
            CreateRequest());

        var createdReader = await createResponse.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(createdReader);

        var response = await client.PutAsJsonAsync(
            $"/api/readers/{createdReader.Id}",
            UpdateRequest());

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await client.GetAsync(
            $"/api/readers/{createdReader.Id}");

        var updatedReader = await getResponse.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(updatedReader);
        Assert.Equal("Petro", updatedReader.FirstName);
        Assert.Equal("Ivanenko", updatedReader.LastName);
    }

    [Fact]
    public async Task Update_WhenReaderDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/readers/999",
            UpdateRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenReaderExists_ReturnsNoContent()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/readers",
            CreateRequest());

        var createdReader = await createResponse.Content
            .ReadFromJsonAsync<ReaderResponse>();

        Assert.NotNull(createdReader);

        var response = await client.DeleteAsync(
            $"/api/readers/{createdReader.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await client.GetAsync(
            $"/api/readers/{createdReader.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenReaderDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/readers/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
