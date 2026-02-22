using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Data.EntityMapping
{
    public class PlatformMapping : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.ToTable("Platform");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(p => p.Name)
                   .IsUnique();
        }
    }
}
