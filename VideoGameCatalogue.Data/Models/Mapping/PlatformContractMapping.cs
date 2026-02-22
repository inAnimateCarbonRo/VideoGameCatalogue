using System.Collections.Generic;
using System.Linq;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Models.Mapping
{
    public static class PlatformContractMapping
    {
        public static Platform MapToEntity(this CreatePlatformRequest request)
        {
            return new Platform
            {
                Name = request.Name
            };
        }

        public static Platform MapToEntity(this UpdatePlatformRequest request, int id)
        {
            return new Platform
            {
                Id = id,
                Name = request.Name
            };
        }

        public static PlatformResponse MapToResponse(this Platform entity)
        {
            return new PlatformResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static PlatformsResponse MapToResponse(this IEnumerable<Platform> entities)
        {
            return new PlatformsResponse
            {
                Items = entities.Select(e => e.MapToResponse()).ToList()
            };
        }
    }
}