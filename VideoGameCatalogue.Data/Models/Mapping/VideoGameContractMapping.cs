using System;
using System.Collections.Generic;
using System.Linq;
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
                UserScore = request.UserScore,

                PublisherId = request.PublisherId,
                DeveloperId = request.DeveloperId,

                // Cover image bytes are handled in service (Base64 -> byte[])
                // so we don't set CoverImageBytes here.
                CoverImageContentType = request.CoverImageContentType
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
                UserScore = request.UserScore,

                PublisherId = request.PublisherId,
                DeveloperId = request.DeveloperId,

                // Cover image bytes are handled in service (Base64 -> byte[])
                CoverImageContentType = request.CoverImageContentType
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
                UserScore = entity.UserScore,

                Genres = entity.Genres.Select(g => new GenreResponse
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),

                Platforms = entity.Platforms.Select(p => new PlatformResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList(),

                Publisher = entity.Publisher == null
                    ? null
                    : new CompanyResponse
                    {
                        Id = entity.Publisher.Id,
                        Name = entity.Publisher.Name
                    },

                Developer = entity.Developer == null
                    ? null
                    : new CompanyResponse
                    {
                        Id = entity.Developer.Id,
                        Name = entity.Developer.Name
                    },

                CoverImageBase64 = entity.CoverImageBytes == null
                    ? null
                    : Convert.ToBase64String(entity.CoverImageBytes),

                CoverImageContentType = entity.CoverImageContentType
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