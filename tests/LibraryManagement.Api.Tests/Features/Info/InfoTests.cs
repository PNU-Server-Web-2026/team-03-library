using System.Net;
using System.Net.Http.Json;
using LibraryManagement.Api.Features.Info;

namespace LibraryManagement.Api.Tests.Features.Info;

public class InfoTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Get_Info_ReturnsApplicationMetadata()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<InfoResponse>();

        Assert.NotNull(body);
        Assert.Equal("LibraryManagement.Api", body.Name);
        Assert.False(string.IsNullOrWhiteSpace(body.Version));
        Assert.False(string.IsNullOrWhiteSpace(body.Environment));

        Assert.InRange(
            body.UtcNow,
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(1)
        );
    }
}
