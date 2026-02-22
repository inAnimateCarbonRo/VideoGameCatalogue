using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Responses
{
    public class VideoGameResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public int UserScore { get; set; } // Max value of 100

        public List<GenreResponse> Genres { get; set; } = new();
    }

    public class VideoGamesResponse
    {
        public List<VideoGameResponse> Items { get; set; } = new();
    }
}
