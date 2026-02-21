using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VideoGameCatalogue.Shared.Entities;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class VideoGame : EntitySoftDeleteBase
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Synopsis { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public decimal UserScore { get; set; } // Max value of 100

        //Wamt to add
        //Platforms (List of another model)
        //Genres (List of another model)
        //Publisher (List of another model)
        //Developer (List of another model) 

        // Image in DB
        //public byte[]? CoverImageBytes { get; set; }
        //public string? CoverImageContentType { get; set; } // "image/jpeg"
    }
}
