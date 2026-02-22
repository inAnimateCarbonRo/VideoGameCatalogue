using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Services
{
    public interface IPlatformService : IServiceBase<Platform>
    {
    }

    public class PlatformService : ServiceBase<Platform>, IPlatformService
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformService(IPlatformRepository repository) : base(repository)
        {
            _platformRepository = repository;
        }
    }
}
