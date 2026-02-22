using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Responses
{
    public class PlatformResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class PlatformsResponse
    {
        public List<PlatformResponse> Items { get; set; } = new();
    }
}
