using System;
using System.Collections.Generic;

namespace VideoGameCatalogue.Data.Models.Contracts.Responses
{
    public class VideoGameResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Synopsis { get; set; } = string.Empty;

        public DateOnly ReleaseDate { get; set; }

        public int UserScore { get; set; }

        // Relationships
        public List<GenreResponse> Genres { get; set; } = new();

        public List<PlatformResponse> Platforms { get; set; } = new();

        public CompanyResponse? Publisher { get; set; }

        public CompanyResponse? Developer { get; set; }

        // Cover image (Base64 for Angular)
        public string? CoverImageBase64 { get; set; }

        public string? CoverImageContentType { get; set; }
    }

    public class VideoGamesResponse
    {
        public List<VideoGameResponse> Items { get; set; } = new();
    }
}