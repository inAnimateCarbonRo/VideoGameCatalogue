using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text.Json.Nodes;
using VideoGameCatalogue.BusinessLogic;
using VideoGameCatalogue.BusinessLogic.Context;
using VideoGameCatalogue.Shared.Config;
using VideoGameCatalogue.Shared.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    //Adding this because i want a default exmaple for DateOnly, 
    //purely convenice
    options.AddSchemaTransformer((schema, context, ct) =>
    {
        var type = context.JsonTypeInfo.Type;

        if (type == typeof(DateOnly) || type == typeof(DateOnly?))
        {
            // ✅ Built-in OpenAPI uses JsonSchemaType enum
            schema.Type = JsonSchemaType.String;
            schema.Format = "date";

            var today = DateOnly.FromDateTime(DateTime.UtcNow)
                .ToString("yyyy-MM-dd");

            schema.Example = JsonValue.Create(today);
            schema.Default = JsonValue.Create(today);
        }

        return Task.CompletedTask;
    });
});

// Get the DB connection string
var dbConnection = builder.Configuration.GetConnectionString(
    EnumUtilities.GetEnumDescription(SystemConfig.CurrentSystemEnum))
    ?? throw new InvalidOperationException("Database connection string is not configured.");

SystemDbContext.SQLConnectionString(dbConnection);
builder.Services.AddDbContext();
builder.Services.AddVideoGameCatalogueServices(); // DI for business logic services and repositories


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
        .WithTitle("VideoGameCatalogue API")
        .WithTheme(ScalarTheme.Mars)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .EnableDarkMode();
    }
   );
   // i like scalar, taken from this video: https://www.youtube.com/watch?v=8yI4gD1HruY&t=316s
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Convenience step, i want to load scalar when the project starts
// redirect "/" to Scalar UI so devs land on docs by default.
//https://blog.antosubash.com/posts/dotnet-openapi-with-scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1"))
   .ExcludeFromDescription();

app.Run();
