using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface IGenreRepository : IRepositoryBase<Genre>
    {

    }
    public class GenreRepository : RepositoryBase<Genre>, IGenreRepository
    {

        public GenreRepository(VideoGameCatalogueContext dbContext) : base(dbContext) { }

    }

}
