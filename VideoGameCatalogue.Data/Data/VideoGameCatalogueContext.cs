using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Data.Data.EntityMapping;

namespace VideoGameCatalogue.Data.Data
{
    public class VideoGameCatalogueContext : DbContext
    {

        public VideoGameCatalogueContext(DbContextOptions<VideoGameCatalogueContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }


        // DbSets (Tables) 
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();  //gets a non nullable DbSet 
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.ApplyConfiguration(new VideoGameMapping());
            modelBuilder.ApplyConfiguration(new GenreMapping());
            modelBuilder.ApplyConfiguration(new PlatformMapping());
            modelBuilder.ApplyConfiguration(new CompanyMapping());
        }
    }
}
