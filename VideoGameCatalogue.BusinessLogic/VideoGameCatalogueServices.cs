using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.BusinessLogic.Services;

namespace VideoGameCatalogue.BusinessLogic
{
    public static class VideoGameCatalogueServices
    {

        public static IServiceCollection AddVideoGameCatalogueServices(this IServiceCollection services)
        {
            
            services.AddScoped<IVideoGameService, VideoGameService>();
            services.AddScoped<IVideoGameRepository, VideoGameRepository>();
            
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IGenreRepository, GenreRepository>();

            return services;
        }
    }
}
