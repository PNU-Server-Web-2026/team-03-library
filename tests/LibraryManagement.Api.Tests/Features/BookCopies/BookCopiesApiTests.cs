using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LibraryManagement.Api.Features.BookCopies;

namespace LibraryManagement.Api.Tests.Features.BookCopies;

public class BookCopiesApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Url = "/api/book-copies";
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateAsync();

        var response = await _client.GetAsync(Url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var copies = await response.Content.ReadFromJsonAsync<List<BookCopyResponse>>();
        Assert.NotNull(copies);
        Assert.Contains(created, copies);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var created = await CreateAsync();

        var response = await _client.GetAsync($"{Url}/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<BookCopyResponse>());
    }

    [Fact]
    public async Task GetById_Missing_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"{Url}/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocation()
    {
        var request = new CreateBookCopyRequest("INV-000123", BookCopyCondition.New, "A-1");

        var response = await _client.PostAsJsonAsync(Url, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<BookCopyResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("INV-000123", created.InventoryNumber);
        Assert.Equal(BookCopyCondition.New, created.Condition);
        Assert.Equal("A-1", created.Location);

        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith($"{Url}/{created.Id}", response.Headers.Location.ToString());
        var byLocation = await _client.GetFromJsonAsync<BookCopyResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Post_SerializesConditionAsString()
    {
        var response = await _client.PostAsJsonAsync(
            Url, new CreateBookCopyRequest("INV-000124", BookCopyCondition.Worn, "B-2"));

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Worn", json.RootElement.GetProperty("condition").GetString());
    }

    [Fact]
    public async Task Put_Existing_ReturnsNoContentAndUpdates()
    {
        var created = await CreateAsync();
        var request = new UpdateBookCopyRequest("INV-000999", BookCopyCondition.Damaged, "C-3");

        var response = await _client.PutAsJsonAsync($"{Url}/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await _client.GetFromJsonAsync<BookCopyResponse>($"{Url}/{created.Id}");
        Assert.Equal(
            new BookCopyResponse(created.Id, "INV-000999", BookCopyCondition.Damaged, "C-3"),
            updated);
    }

    [Fact]
    public async Task Put_Missing_ReturnsNotFound()
    {
        var request = new UpdateBookCopyRequest("INV-000999", BookCopyCondition.Good, "C-3");

        var response = await _client.PutAsJsonAsync($"{Url}/{int.MaxValue}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContentAndRemoves()
    {
        var created = await CreateAsync();

        var response = await _client.DeleteAsync($"{Url}/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var after = await _client.GetAsync($"{Url}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, after.StatusCode);
    }

    [Fact]
    public async Task Delete_Missing_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"{Url}/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<BookCopyResponse> CreateAsync()
    {
        var response = await _client.PostAsJsonAsync(
            Url, new CreateBookCopyRequest("INV-000001", BookCopyCondition.Good, "A-1"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<BookCopyResponse>())!;
    }
}
