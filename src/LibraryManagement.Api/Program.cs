using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Api.Features.Authors;
using LibraryManagement.Api.Features.Cors;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddCorsFeature(builder.Configuration);
builder.Services.AddAuthorsFeature();
builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Library Management API";
        document.Info.Version = "v1";
        document.Info.Description = "API для керування бібліотекою та книгами.";
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// HTTP-конвеєр.
if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar");
}
app.UseCorsFeature();
app.MapControllers();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;
