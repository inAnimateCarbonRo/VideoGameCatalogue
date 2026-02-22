using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.BusinessLogic.Services;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Shared.Endpoints;
using VideoGameCatalogue.Data.Models.Mapping;

namespace VideoGameCatalogue.Api.Controllers
{
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _service;

        public GenresController(IGenreService service)
        {
            _service = service;
        }

        [HttpGet(ApiEndpoints.GenreEndpoints.GetAll)]
        [ProducesResponseType(typeof(GenresResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GenresResponse>> GetAll(CancellationToken token)
        {
            var items = await _service.GetAllAsync(token);
            var response = items.MapToResponse();

            return Ok(response);
        }

        [HttpGet(ApiEndpoints.GenreEndpoints.Get)]
        [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenreResponse>> GetById([FromRoute] int id, CancellationToken token)
        {
            var item = await _service.GetByIdAsync(id, token);
            if (item == null) return NotFound();

            return Ok(item.MapToResponse());
        }

        [HttpGet(ApiEndpoints.GenreEndpoints.GetAllIncludingDeleted)]
        [ProducesResponseType(typeof(GenresResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GenresResponse>> GetAllIncludingDeleted(CancellationToken token)
        {
            var items = await _service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();

            return Ok(response);
        }

        [HttpPost(ApiEndpoints.GenreEndpoints.Create)]
        [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GenreResponse>> Create(
            [FromBody] CreateGenreRequest item,
            CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");

            var entity = item.MapToEntity();

            await _service.AddWithReturningEntityAsync(entity, token);

            var response = entity.MapToResponse();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
        }

        [HttpPut(ApiEndpoints.GenreEndpoints.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateGenreRequest item,
            CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");
            if (id != item.Id) return BadRequest("Route id does not match payload id.");

            var entity = item.MapToEntity(id);

            var updated = await _service.UpdateAsync(entity, token);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete(ApiEndpoints.GenreEndpoints.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token)
        {
            var deleted = await _service.DeleteAsync(id, token);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPut(ApiEndpoints.GenreEndpoints.Restore)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore([FromRoute] int id, CancellationToken token)
        {
            var restored = await _service.RestoreAsync(id, token);
            if (!restored) return NotFound();

            return NoContent();
        }

        [HttpDelete(ApiEndpoints.GenreEndpoints.FullDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FullDelete([FromRoute] int id, CancellationToken token)
        {
            var deleted = await _service.FullDeleteAsync(id, token);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}