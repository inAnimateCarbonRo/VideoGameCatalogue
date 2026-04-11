using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Services;
using VideoGameCatalogue.Models.Models.Contracts.Requests;
using VideoGameCatalogue.Models.Models.Contracts.Responses;
using VideoGameCatalogue.Models.Models.Mapping;
using VideoGameCatalogue.Shared.Endpoints;

namespace VideoGameCatalogue.Api.Endpoints
{
    public static class GenreEndpoints
    {
        public static void MapGenreEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(ApiEndpoints.GenreEndpoints.GetAll.Split('/')[..2].Aggregate((a, b) => $"{a}/{b}"))
                .WithTags("Genres");

            group.MapGet("/", GetAll)
                .WithName("GetAllGenres")
            
                .Produces<GenresResponse>(StatusCodes.Status200OK);

            group.MapGet("/{id}", GetById)
                .WithName("GetGenreById")
            
                .Produces<GenreResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/all-including-deleted", GetAllIncludingDeleted)
                .WithName("GetAllGenresIncludingDeleted")
            
                .Produces<GenresResponse>(StatusCodes.Status200OK);

            group.MapPost("/", Create)
                .WithName("CreateGenre")
            
                .Produces<GenreResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status409Conflict);

            group.MapPut("/{id}", Update)
                .WithName("UpdateGenre")
            
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", Delete)
                .WithName("DeleteGenre")
            
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/restore/{id}", Restore)
                .WithName("RestoreGenre")
            
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/fulldelete/{id}", FullDelete)
                .WithName("FullDeleteGenre")
            
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetAll(IGenreService service, CancellationToken token)
        {
            var items = await service.GetAllAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> GetById(int id, IGenreService service, CancellationToken token)
        {
            var item = await service.GetByIdAsync(id, token);
            if (item == null) return Results.NotFound();

            return Results.Ok(item.MapToResponse());
        }

        private static async Task<IResult> GetAllIncludingDeleted(IGenreService service, CancellationToken token)
        {
            var items = await service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();
            return Results.Ok(response);
        }

        private static async Task<IResult> Create([FromBody] CreateGenreRequest item, IGenreService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");

            try
            {
                var entity = item.MapToEntity();
                await service.AddWithReturningEntityAsync(entity, token);
                return Results.CreatedAtRoute("GetGenreById", new { id = entity.Id }, entity.MapToResponse());
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                                              && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Results.Conflict($"Genre '{item.Name}' already exists.");
            }
        }

        private static async Task<IResult> Update(int id, [FromBody] UpdateGenreRequest item, IGenreService service, CancellationToken token)
        {
            if (item == null) return Results.BadRequest("Invalid data.");
            if (id != item.Id) return Results.BadRequest("Route id does not match payload id.");

            var entity = item.MapToEntity(id);
            var updated = await service.UpdateAsync(entity, token);

            if (!updated) return Results.NotFound();
            return Results.NoContent();
        }

        private static async Task<IResult> Delete(int id, IGenreService service, CancellationToken token)
        {
            var deleted = await service.DeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> Restore(int id, IGenreService service, CancellationToken token)
        {
            var restored = await service.RestoreAsync(id, token);
            if (!restored) return Results.NotFound();

            return Results.NoContent();
        }

        private static async Task<IResult> FullDelete(int id, IGenreService service, CancellationToken token)
        {
            var deleted = await service.FullDeleteAsync(id, token);
            if (!deleted) return Results.NotFound();

            return Results.NoContent();
        }
    }
}
