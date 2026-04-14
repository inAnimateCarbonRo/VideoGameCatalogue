using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Services;
using VideoGameCatalogue.Models.Models.Contracts.Requests;
using VideoGameCatalogue.Models.Models.Contracts.Responses;
using VideoGameCatalogue.Models.Models.Mapping;
using VideoGameCatalogue.Shared.Endpoints;

namespace VideoGameCatalogue.Api.Endpoints
{
    public static class PlatformEndpoints
    {
        public static void MapPlatformEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(ApiEndpoints.PlatformEndpoints.GetAll.Split('/')[..2].Aggregate((a, b) => $"{a}/{b}"))
                .WithTags("Platforms");

            group.MapGet("/", GetAll)
                .WithName("GetAllPlatforms")
          
                .Produces<PlatformsResponse>(StatusCodes.Status200OK);

            group.MapGet("/{id}", GetById)
                .WithName("GetPlatformById")
          
                .Produces<PlatformResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/all-including-deleted", GetAllIncludingDeleted)
                .WithName("GetAllPlatformsIncludingDeleted")
          
                .Produces<PlatformsResponse>(StatusCodes.Status200OK);

            group.MapPost("/", Create)
                .WithName("CreatePlatform")
          
                .Produces<PlatformResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPut("/{id}", Update)
                .WithName("UpdatePlatform")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", Delete)
                .WithName("DeletePlatform")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/restore/{id}", Restore)
                .WithName("RestorePlatform")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/fulldelete/{id}", FullDelete)
                .WithName("FullDeletePlatform")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetAll(IPlatformService service, CancellationToken token)
        {
            var items = await service.GetAllAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> GetById(int id, IPlatformService service, CancellationToken token)
        {
            var item = await service.GetByIdAsync(id, token);
            if (item == null) return Results.NotFound();

            return Results.Ok(item.MapToResponse());
        }

        private static async Task<IResult> GetAllIncludingDeleted(IPlatformService service, CancellationToken token)
        {
            var items = await service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> Create([FromBody] CreatePlatformRequest item, IPlatformService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");

            try
            {
                var entity = item.MapToEntity();
                await service.AddWithReturningEntityAsync(entity, token);
                return Results.CreatedAtRoute("GetPlatformById", new { id = entity.Id }, entity.MapToResponse());
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                                              && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Results.Conflict($"Platform '{item.Name}' already exists.");
            }
        }

        private static async Task<IResult> Update(int id, [FromBody] UpdatePlatformRequest item, IPlatformService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");
            if (id != item.Id) return Results.BadRequest("Route id does not match payload id.");

            var entity = item.MapToEntity(id);
            var updated = await service.UpdateAsync(entity, token);

            if (!updated) return Results.NotFound();
            return Results.NoContent();
        }

        private static async Task<IResult> Delete(int id, IPlatformService service, CancellationToken token)
        {
            var deleted = await service.DeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> Restore(int id, IPlatformService service, CancellationToken token)
        {
            var restored = await service.RestoreAsync(id, token);
            if (!restored) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> FullDelete(int id, IPlatformService service, CancellationToken token)
        {
            var deleted = await service.FullDeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }
    }
}
