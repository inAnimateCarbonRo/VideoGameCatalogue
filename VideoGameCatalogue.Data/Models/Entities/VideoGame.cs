using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class VideoGame : EntityBase
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }
        [Required]
        [MaxLength(1000)]
        public required string Synopsis { get; set; }
        public required DateOnly ReleaseDate { get; set; }

        [Range(0, 100)]
        public int UserScore { get; set; } // Max value of 100


        // A VideoGame can have multiple Genres
        public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();

        //Wamt to add
        //Platforms (List of another model)
        //Genres (List of another model)
        //Publisher (List of another model) // ThirdParty
        //Developer (List of another model) 

        // Image in DB
        //public byte[]? CoverImageBytes { get; set; }
        //public string? CoverImageContentType { get; set; } // "image/jpeg"
    }
}
