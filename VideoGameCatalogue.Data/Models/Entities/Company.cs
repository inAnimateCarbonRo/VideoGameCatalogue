using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class Company : EntityBase
    {
        [Required, MaxLength(200)]
        public required string Name { get; set; }

        // Two separate relationships to VideoGame
        public ICollection<VideoGame> PublishedGames { get; set; } = new HashSet<VideoGame>();
        public ICollection<VideoGame> DevelopedGames { get; set; } = new HashSet<VideoGame>();
    }
}
