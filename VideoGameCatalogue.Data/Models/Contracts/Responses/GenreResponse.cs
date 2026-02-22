using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Responses
{
    public class GenreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }

    public class GenresResponse
    {
        public List<GenreResponse> Items { get; set; } = new();
    }
}
