using VideoGameCatalogue.Api.Endpoints;
using VideoGameCatalogue.Api.Extensions;
using VideoGameCatalogue.BusinessLogic;
using VideoGameCatalogue.BusinessLogic.Context;
using VideoGameCatalogue.Shared.Config;
using VideoGameCatalogue.Shared.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiConfiguration();

// Get the DB connection string
var dbConnection = builder.Configuration.GetConnectionString(
    EnumUtilities.GetEnumDescription(SystemConfig.CurrentSystemEnum))
    ?? throw new InvalidOperationException("Database connection string is not configured.");

SystemDbContext.SQLConnectionString(dbConnection);
builder.Services.AddDbContext();


builder.Services.AddAuthorization();
builder.Services.AddVideoGameCatalogueServices(); // DI for business logic services and repositories


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApiEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

// Map minimal API endpoints
app.MapApiEndpoints();

app.Run();
