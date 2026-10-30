namespace LibraryManagement.Api.Features.Info;

public sealed record InfoResponse(
    string Name,
    string Version,
    string Environment,
    DateTime UtcNow);
