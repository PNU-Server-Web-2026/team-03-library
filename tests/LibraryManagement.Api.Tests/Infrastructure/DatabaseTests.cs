using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Api.Tests.Infrastructure;

[Collection(ApiCollection.Name)]
public class DatabaseTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task AppDbContext_UsesPostgreSqlContainer()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.True(await db.Database.CanConnectAsync());
        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", db.Database.ProviderName);
        Assert.Equal(Factory.ConnectionString, db.Database.GetConnectionString());
    }

    [Fact]
    public async Task Migrations_AreApplied()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var pending = await db.Database.GetPendingMigrationsAsync();

        Assert.Empty(pending);
    }
}
