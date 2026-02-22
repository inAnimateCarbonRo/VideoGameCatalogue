using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using VideoGameCatalogue.Shared.Base;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class VideoGame : EntityBase
    {
        [Required, MaxLength(200)]
        public required string Title { get; set; }

        [Required, MaxLength(1000)]
        public required string Synopsis { get; set; }

        public required DateOnly ReleaseDate { get; set; }

        [Range(0, 100)]
        public int UserScore { get; set; } // Max 100


        public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
        public ICollection<Platform> Platforms { get; set; } = new HashSet<Platform>();
         
        public int? PublisherId { get; set; }
        public Company? Publisher { get; set; }

        public int? DeveloperId { get; set; }
        public Company? Developer { get; set; }

        // Cover image stored in DB
        public byte[]? CoverImageBytes { get; set; }
        [MaxLength(100)]
        public string? CoverImageContentType { get; set; } // "image/jpeg", "image/png"
    }
}
