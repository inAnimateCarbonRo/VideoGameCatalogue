using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Services
{
 

    public interface IVideoGameService : IServiceBase<VideoGame>
    {

    }


    public class VideoGameService : ServiceBase<VideoGame>, IVideoGameService
    {
        private readonly IVideoGameRepository _VideoGameRepository;

        public VideoGameService(IVideoGameRepository repository) : base(repository)
        {
            _VideoGameRepository = repository;
        }

    }

}
