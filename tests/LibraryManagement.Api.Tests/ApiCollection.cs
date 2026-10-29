namespace LibraryManagement.Api.Tests;

/// <summary>
/// Колекція інтеграційних тестів: усі класи з <c>[Collection(ApiCollection.Name)]</c>
/// використовують один екземпляр <see cref="ApiFactory"/>, тобто один контейнер PostgreSQL.
/// Тести всередині колекції виконуються послідовно.
/// </summary>
[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Api";
}
