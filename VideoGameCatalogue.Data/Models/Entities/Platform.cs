using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class Platform : EntityBase
    {
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        public ICollection<VideoGame> VideoGames { get; set; } = new HashSet<VideoGame>();
    }
}
