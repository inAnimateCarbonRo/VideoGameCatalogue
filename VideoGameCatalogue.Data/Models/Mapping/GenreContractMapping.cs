using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Models.Mapping
{
    public static class GenreContractMapping
    {
        public static Genre MapToEntity(this CreateGenreRequest request)
        {
            return new Genre
            {
                Name = request.Name
            };
        }

        public static Genre MapToEntity(this UpdateGenreRequest request, int id)
        {
            return new Genre
            {
                Id = id,
                Name = request.Name
            };
        }

        public static GenreResponse MapToResponse(this Genre entity)
        {
            return new GenreResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static GenresResponse MapToResponse(this IEnumerable<Genre> entities)
        {
            return new GenresResponse
            {
                Items = entities.Select(e => e.MapToResponse()).ToList()
            };
        }
    }
}
