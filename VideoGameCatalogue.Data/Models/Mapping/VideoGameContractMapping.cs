using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Models.Mapping
{
    public static class VideoGameContractMapping
    {
        public static VideoGame MapToEntity(this CreateVideoGameRequest request)
        {
            return new VideoGame
            {
                Title = request.Title,
                Synopsis = request.Synopsis,
                ReleaseDate = request.ReleaseDate,
                UserScore = request.UserScore
            };
        }

        public static VideoGame MapToEntity(this UpdateVideoGameRequest request, int id)
        {
            return new VideoGame
            {
                Id = id, 
                Title = request.Title,
                Synopsis = request.Synopsis,
                ReleaseDate = request.ReleaseDate,
                UserScore = request.UserScore
            };
        }

        public static VideoGameResponse MapToResponse(this VideoGame entity)
        {
            return new VideoGameResponse
            {
                Id = entity.Id, 
                Title = entity.Title,
                Synopsis = entity.Synopsis,
                ReleaseDate = entity.ReleaseDate,
                UserScore = entity.UserScore
            };
        }

        public static VideoGamesResponse MapToResponse(this IEnumerable<VideoGame> entities)
        {
            return new VideoGamesResponse
            {
                Items = entities.Select(e => e.MapToResponse()).ToList()
            };
        }
    }
}
