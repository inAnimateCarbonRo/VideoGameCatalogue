using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Models.Models.Contracts.Requests
{
    public class UpdateGenreRequest : CreateGenreRequest
    {
        public int Id { get; set; }
    }
}
