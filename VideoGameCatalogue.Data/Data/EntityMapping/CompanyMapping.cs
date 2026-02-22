using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Data.EntityMapping
{
    public class CompanyMapping : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            // Prevent duplicate company names
            builder.HasIndex(c => c.Name)
                   .IsUnique();

            //mapping for the many-to-many relationship with VideoGame for PublishedGames
        }
    }
}
