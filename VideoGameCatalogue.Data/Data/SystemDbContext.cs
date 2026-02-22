using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Data;

namespace VideoGameCatalogue.BusinessLogic.Context
{
    public static class SystemDbContext
    {
        private static string cnnString = string.Empty;

        public static void SQLConnectionString(string connectionString)
        {
            cnnString = connectionString;
        }

        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddDbContextFactory<VideoGameCatalogueContext>(options =>
         options.UseSqlServer(cnnString, cTimeout => cTimeout.CommandTimeout(500)));
        }
    }
}
