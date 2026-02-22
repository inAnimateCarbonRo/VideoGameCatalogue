using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface IPlatformRepository : IRepositoryBase<Platform>
    {
    }

    public class PlatformRepository : RepositoryBase<Platform>, IPlatformRepository
    {
        public PlatformRepository(VideoGameCatalogueContext dbContext)
            : base(dbContext)
        {
        }
    }
}
