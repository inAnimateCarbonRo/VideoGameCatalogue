using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Services
{
 

    public interface IGenreService : IServiceBase<Genre>
    {

    }


    public class GenreService : ServiceBase<Genre>, IGenreService
    {
        private readonly IGenreRepository _GenreRepository;

        public GenreService(IGenreRepository repository) : base(repository)
        {
            _GenreRepository = repository;
        }

    }

}
