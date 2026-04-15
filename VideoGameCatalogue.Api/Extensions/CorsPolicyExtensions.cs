namespace VideoGameCatalogue.Api.Extensions;

public static class CorsPolicyExtensions
{
    private const string CorsPolicy = "VideoGameCataloguePolicy";

    /// <summary>
    /// Adds CORS services with a configurable policy.
    /// Allows all origins in development, restricted origins in production.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // Allow all origins in development for easier testing
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    // In production, configure specific allowed origins from configuration
                    var allowedOrigins = builder.Configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? [];

                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Maps the CORS middleware to the application pipeline.
    /// </summary>
    public static void UseCorsPolicy(this WebApplication app)
    {
        app.UseCors(CorsPolicy);
    }
}
