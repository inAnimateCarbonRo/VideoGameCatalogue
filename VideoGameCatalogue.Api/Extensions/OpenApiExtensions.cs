using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text.Json.Nodes;

namespace VideoGameCatalogue.Api.Extensions;

public static class OpenApiExtensions
{
    /// <summary>
    /// Configures OpenAPI services with custom schema transformers (e.g., DateOnly formatting).
    /// </summary>
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            // Adding this because we want a default example for DateOnly, purely convenience
            options.AddSchemaTransformer((schema, context, ct) =>
            {
                var type = context.JsonTypeInfo.Type;

                if (type == typeof(DateOnly) || type == typeof(DateOnly?))
                {
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

        return services;
    }

    /// <summary>
    /// Maps OpenAPI and Scalar documentation endpoints for development environments.
    /// Redirects "/" to Scalar UI so devs land on docs by default.
    /// </summary>
    public static void MapOpenApiEndpoints(this WebApplication app)
    {
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
            });

            // Redirect "/" to Scalar UI so devs land on docs by default
            // Reference: https://blog.antosubash.com/posts/dotnet-openapi-with-scalar
            app.MapGet("/", () => Results.Redirect("/scalar/v1"))
               .ExcludeFromDescription();
        }
    }
}
