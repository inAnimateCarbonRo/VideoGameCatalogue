using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace VideoGameCatalogue.Data.Data.EntityMapping
{
    public class VideoGameMapping : IEntityTypeConfiguration<VideoGame>
    {
        public void Configure(EntityTypeBuilder<VideoGame> builder)
        {
            builder
                 .ToTable("VideoGame")
                 .HasQueryFilter(a => !a.isDeleted); //Soft Delete items auto excluded

        }
    }
}