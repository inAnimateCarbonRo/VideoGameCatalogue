using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface ICompanyRepository : IRepositoryBase<Company>
    {
    }

    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(VideoGameCatalogueContext dbContext)
            : base(dbContext)
        {
        }
    }
}
