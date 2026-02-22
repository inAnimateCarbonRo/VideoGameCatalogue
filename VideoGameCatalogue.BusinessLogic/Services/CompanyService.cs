using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Services
{
    public interface ICompanyService : IServiceBase<Company>
    {
    }

    public class CompanyService : ServiceBase<Company>, ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository repository) : base(repository)
        {
            _companyRepository = repository;
        }
    }
}
