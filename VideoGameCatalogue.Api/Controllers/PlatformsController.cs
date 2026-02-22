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
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformService _service;

        public PlatformsController(IPlatformService service)
        {
            _service = service;
        }

        [HttpGet(ApiEndpoints.PlatformEndpoints.GetAll)]
        [ProducesResponseType(typeof(PlatformsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlatformsResponse>> GetAll(CancellationToken token)
        {
            var items = await _service.GetAllAsync(token);
            var response = items.MapToResponse();
            return Ok(response);
        }

        [HttpGet(ApiEndpoints.PlatformEndpoints.Get)]
        [ProducesResponseType(typeof(PlatformResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlatformResponse>> GetById([FromRoute] int id, CancellationToken token)
        {
            var item = await _service.GetByIdAsync(id, token);
            if (item == null) return NotFound();

            return Ok(item.MapToResponse());
        }

        [HttpGet(ApiEndpoints.PlatformEndpoints.GetAllIncludingDeleted)]
        [ProducesResponseType(typeof(PlatformsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlatformsResponse>> GetAllIncludingDeleted(CancellationToken token)
        {
            var items = await _service.GetAllIncludingDeletedAsync(token);
            var response = items.MapToResponse();
            return Ok(response);
        }

        [HttpPost(ApiEndpoints.PlatformEndpoints.Create)]
        [ProducesResponseType(typeof(PlatformResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PlatformResponse>> Create([FromBody] CreatePlatformRequest item, CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");

            try
            {
                var entity = item.MapToEntity();
                await _service.AddWithReturningEntityAsync(entity, token);
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.MapToResponse());
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
                                              && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                return Conflict($"Platform '{item.Name}' already exists.");
            }
        }

        [HttpPut(ApiEndpoints.PlatformEndpoints.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdatePlatformRequest item,
            CancellationToken token)
        {
            if (item == null) return BadRequest("Invalid data.");
            if (id != item.Id) return BadRequest("Route id does not match payload id.");

            var entity = item.MapToEntity(id);
            var updated = await _service.UpdateAsync(entity, token);

            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete(ApiEndpoints.PlatformEndpoints.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token)
        {
            var deleted = await _service.DeleteAsync(id, token);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPut(ApiEndpoints.PlatformEndpoints.Restore)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore([FromRoute] int id, CancellationToken token)
        {
            var restored = await _service.RestoreAsync(id, token);
            if (!restored) return NotFound();

            return NoContent();
        }

        [HttpDelete(ApiEndpoints.PlatformEndpoints.FullDelete)]
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