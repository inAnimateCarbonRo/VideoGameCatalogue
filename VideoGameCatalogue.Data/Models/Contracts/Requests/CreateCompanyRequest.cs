using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class CreateCompanyRequest
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }
    }
}
