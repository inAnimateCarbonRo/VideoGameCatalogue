using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.Data.Models.Entities
{
    public class Genre : EntityBase
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<VideoGame> VideoGames { get; set; } = new HashSet<VideoGame>();
    }
}