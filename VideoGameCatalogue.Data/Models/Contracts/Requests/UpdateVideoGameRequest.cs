using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Data.Models.Contracts.Requests
{
    public class UpdateVideoGameRequest:CreateVideoGameRequest
    {
        public int Id { get; set; }
    }
}
