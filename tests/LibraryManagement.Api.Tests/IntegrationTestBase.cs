namespace LibraryManagement.Api.Tests;

/// <summary>
/// Базовий клас інтеграційних тестів: перед кожним тестом очищає БД.
/// Нащадок має бути позначений <c>[Collection(ApiCollection.Name)]</c>.
/// </summary>
public abstract class IntegrationTestBase(ApiFactory factory) : IAsyncLifetime
{
    protected ApiFactory Factory { get; } = factory;

    protected HttpClient Client { get; } = factory.CreateClient();

    public Task InitializeAsync() => Factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
