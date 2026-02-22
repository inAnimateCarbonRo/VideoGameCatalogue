using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Services;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Data.Models.Mapping;
using VideoGameCatalogue.Shared.Endpoints;

namespace VideoGameCatalogue.Api.Controllers
{
    [ApiController]
    public class VideoGamesController : ControllerBase
    {
        private readonly IVideoGameService _service;

        public VideoGamesController(IVideoGameService service)
        {
            _service = service;
        }

        [HttpGet(ApiEndpoints.VideoGameEndpoints.GetAll)]
        [ProducesResponseType(typeof(VideoGamesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<VideoGamesResponse>> GetAll(CancellationToken token)
        {
            var items = await _service.GetAllAsync(token);
            var response = items.MapToResponse();

            return Ok(response);
        }

        [HttpGet(ApiEndpoints.VideoGameEndpoints.Get)]
        [ProducesResponseType(typeof(VideoGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGameResponse>> GetById([FromRoute] int id, CancellationToken token)
        {
            var item = await _service.GetByIdAsync(id, token);
            if (item == null) return NotFound();

            return Ok(item.MapToResponse());
        }

        [HttpGet(ApiEndpoints.VideoGameEndpoints.GetAllIncludingDeleted)]
        [ProducesResponseType(typeof(VideoGamesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<VideoGamesResponse>> GetAllIncludingDeleted(CancellationToken token)
        {
            var items = await _service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();
            return Ok(response);
        }
         
        [HttpPost(ApiEndpoints.VideoGameEndpoints.Create)]
        [ProducesResponseType(typeof(VideoGameResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VideoGameResponse>> Create([FromBody] CreateVideoGameRequest item, CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");

            try
            {
                var created = await _service.AddWithGenresAsync(item, token);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.MapToResponse());
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                                              && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                // SQL Server duplicate key / unique constraint
                return Conflict("A record with the same unique value already exists.");
            }
        }


        [HttpPut(ApiEndpoints.VideoGameEndpoints.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateVideoGameRequest item, CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");
            if (id != item.Id) return BadRequest("Route id does not match payload id.");

            try
            {
                var updated = await _service.UpdateWithGenresAsync(item, token);
                if (updated == null) return NotFound();
                return Ok(updated.MapToResponse());

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpDelete(ApiEndpoints.VideoGameEndpoints.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token)
        {
            // this is just a soft delete, the record will still exist in the database but will be marked as deleted
            var deleted = await _service.DeleteAsync(id, token);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPut(ApiEndpoints.VideoGameEndpoints.Restore)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore([FromRoute] int id, CancellationToken token)
        {
            var restored = await _service.RestoreAsync(id, token);
            if (!restored) return NotFound();

            return NoContent();
        }

        [HttpDelete(ApiEndpoints.VideoGameEndpoints.FullDelete)]
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