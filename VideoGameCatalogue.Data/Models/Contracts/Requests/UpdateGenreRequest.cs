using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class UpdateGenreRequest : CreateGenreRequest
    {
        public int Id { get; set; }
    }
}
