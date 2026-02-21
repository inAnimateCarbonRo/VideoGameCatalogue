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
            Database.EnsureCreated();
        }


        // DbSets (Tables) 
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();  //gets a non nullable DbSet 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Singular Table naming 
            modelBuilder.ApplyConfiguration(new VideoGameMapping());
        }
    }
}
