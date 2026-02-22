using System.Collections.Generic;
using System.Linq;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Contracts.Responses;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Models.Mapping
{
    public static class CompanyContractMapping
    {
        public static Company MapToEntity(this CreateCompanyRequest request)
        {
            return new Company
            {
                Name = request.Name
            };
        }

        public static Company MapToEntity(this UpdateCompanyRequest request, int id)
        {
            return new Company
            {
                Id = id,
                Name = request.Name
            };
        }

        public static CompanyResponse MapToResponse(this Company entity)
        {
            return new CompanyResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static CompanysResponse MapToResponse(this IEnumerable<Company> entities)
        {
            return new CompanysResponse
            {
                Items = entities.Select(e => e.MapToResponse()).ToList()
            };
        }
    }
}