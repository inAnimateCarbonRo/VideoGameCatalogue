using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class UpdateCompanyRequest : CreateCompanyRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
