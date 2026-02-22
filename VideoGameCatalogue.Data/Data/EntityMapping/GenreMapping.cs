using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Data.EntityMapping
{
    public class GenreMapping : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder
                .ToTable("Genre");

            builder
                .HasKey(g => g.Id);

            builder
                .Property(g => g.Name)
                .HasColumnType("nvarchar(100)")
                .IsRequired();

            // prevent duplicate genre names
            builder
                .HasIndex(g => g.Name)
                .IsUnique(); 
        }
    }
}