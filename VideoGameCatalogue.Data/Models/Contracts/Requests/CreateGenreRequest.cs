using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class CreateGenreRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

    }
}
