
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LibraryManagement.Api.Features.Books.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LibraryManagement.Api.Tests.Features.Books;

public sealed class BooksApiTests
{
    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>();
    }

    private static CreateBookRequest CreateBookRequest() =>
        new(
            Title: "Clean Code",
            Isbn: "9780132350884",
            PublishYear: 2008,
            Pages: 464,
            Language: "en");

    private static UpdateBookRequest UpdateBookRequest() =>
        new(
            Title: "Clean Code: Updated Edition",
            Isbn: "9780132350884",
            PublishYear: 2009,
            Pages: 500,
            Language: "en");

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/books");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var books = await response.Content
            .ReadFromJsonAsync<List<BookResponse>>();

        Assert.NotNull(books);
    }

    [Fact]
    public async Task GetById_WhenBookExists_ReturnsOk()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/books",
            CreateBookRequest());

        createResponse.EnsureSuccessStatusCode();

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var response = await client.GetAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var book = await response.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal("Clean Code", book.Title);
    }

    [Fact]
    public async Task GetById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/books/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeader()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/books",
            CreateBookRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var book = await response.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);
        Assert.True(book.Id > 0);
        Assert.Equal("Clean Code", book.Title);

        Assert.EndsWith(
            $"/api/books/{book.Id}",
            response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Update_WhenBookExists_ReturnsNoContent()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/books",
            CreateBookRequest());

        createResponse.EnsureSuccessStatusCode();

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/books/{createdBook.Id}",
            UpdateBookRequest());

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // Перевіряємо, чи дані справді оновилися.
        var getResponse = await client.GetAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updatedBook = await getResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(updatedBook);
        Assert.Equal("Clean Code: Updated Edition", updatedBook.Title);
        Assert.Equal(2009, updatedBook.PublishYear);
        Assert.Equal(500, updatedBook.Pages);
    }

    [Fact]
    public async Task Update_WhenBookDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/books/999",
            UpdateBookRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenBookExists_ReturnsNoContent()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/books",
            CreateBookRequest());

        createResponse.EnsureSuccessStatusCode();

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var deleteResponse = await client.DeleteAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // Після видалення книга більше не повинна існувати.
        var getResponse = await client.GetAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenBookDoesNotExist_ReturnsNotFound()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/books/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
