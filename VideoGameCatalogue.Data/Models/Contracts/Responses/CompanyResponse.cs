using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Responses
{
    public class CompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class CompanysResponse
    {
        public List<CompanyResponse> Items { get; set; } = new();
    }
}
