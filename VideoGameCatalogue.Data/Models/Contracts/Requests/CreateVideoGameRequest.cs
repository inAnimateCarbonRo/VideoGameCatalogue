using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class CreateVideoGameRequest
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public required string Synopsis { get; set; }

        public required DateOnly ReleaseDate { get; set; }

        [Range(0, 100)]
        public int UserScore { get; set; }

        //Relationships
        [MinLength(1)]
        public List<int> GenreIds { get; set; } = new();


    }
}
