using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Services;
using VideoGameCatalogue.Models.Models.Contracts.Requests;
using VideoGameCatalogue.Models.Models.Contracts.Responses;
using VideoGameCatalogue.Models.Models.Mapping;
using VideoGameCatalogue.Shared.Endpoints;

namespace VideoGameCatalogue.Api.Endpoints
{
    public static class VideoGameEndpoints
    {
        public static void MapVideoGameEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(ApiEndpoints.VideoGameEndpoints.GetAll.Split('/')[..2].Aggregate((a, b) => $"{a}/{b}"))
                .WithTags("VideoGames");

            group.MapGet("/", GetAll)
                .WithName("GetAllVideoGames")
          
                .Produces<VideoGamesResponse>(StatusCodes.Status200OK);

            group.MapGet("/{id}", GetById)
                .WithName("GetVideoGameById")
          
                .Produces<VideoGameResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/all-including-deleted", GetAllIncludingDeleted)
                .WithName("GetAllVideoGamesIncludingDeleted")
          
                .Produces<VideoGamesResponse>(StatusCodes.Status200OK);

            group.MapPost("/", Create)
                .WithName("CreateVideoGame")
          
                .Produces<VideoGameResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPut("/{id}", Update)
                .WithName("UpdateVideoGame")
          
                .Produces<VideoGameResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete("/{id}", Delete)
                .WithName("DeleteVideoGame")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/restore/{id}", Restore)
                .WithName("RestoreVideoGame")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/fulldelete/{id}", FullDelete)
                .WithName("FullDeleteVideoGame")
          
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetAll(IVideoGameService service, CancellationToken token)
        {
            var items = await service.GetAllAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> GetById(int id, IVideoGameService service, CancellationToken token)
        {
            var item = await service.GetByIdAsync(id, token);
            if (item == null) return Results.NotFound();

            return Results.Ok(item.MapToResponse());
        }

        private static async Task<IResult> GetAllIncludingDeleted(IVideoGameService service, CancellationToken token)
        {
            var items = await service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> Create([FromBody] CreateVideoGameRequest item, IVideoGameService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");

            try
            {
                var created = await service.AddWithRelationshipsAsync(item, token);
                return Results.CreatedAtRoute("GetVideoGameById", new { id = created.Id }, created.MapToResponse());
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                                              && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Results.Conflict("A record with the same unique value already exists.");
            }
        }

        private static async Task<IResult> Update(int id, [FromBody] UpdateVideoGameRequest item, IVideoGameService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");
            if (id != item.Id) return Results.BadRequest("Route id does not match payload id.");

            try
            {
                var updated = await service.UpdateWithRelationshipsAsync(item, token);
                if (updated == null) return Results.NotFound();
                return Results.Ok(updated.MapToResponse());
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

        private static async Task<IResult> Delete(int id, IVideoGameService service, CancellationToken token)
        {
            var deleted = await service.DeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> Restore(int id, IVideoGameService service, CancellationToken token)
        {
            var restored = await service.RestoreAsync(id, token);
            if (!restored) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> FullDelete(int id, IVideoGameService service, CancellationToken token)
        {
            var deleted = await service.FullDeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }
    }
}
