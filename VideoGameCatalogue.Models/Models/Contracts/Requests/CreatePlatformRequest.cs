using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Models.Models.Contracts.Requests
{
    public class CreatePlatformRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
    }
}
