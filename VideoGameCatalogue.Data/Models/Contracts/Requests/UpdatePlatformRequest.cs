using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class UpdatePlatformRequest :CreatePlatformRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
