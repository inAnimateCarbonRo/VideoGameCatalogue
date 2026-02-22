using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.Data.Data.EntityMapping
{
    public class VideoGameMapping : IEntityTypeConfiguration<VideoGame>
    {
        public void Configure(EntityTypeBuilder<VideoGame> builder)
        {
            builder
                 .ToTable("VideoGame")
                 .HasQueryFilter(a => !a.isDeleted); //Soft Delete items auto excluded

            builder
                .HasKey(VideoGame => VideoGame.Id);

            builder
                .Property(VideoGame => VideoGame.Title)
                .HasColumnType("nvarchar(200)")
                .IsRequired();

            builder
               .Property(VideoGame => VideoGame.Synopsis)
               .HasColumnType("nvarchar(1000)")
               .IsRequired();
             
            builder
                .Property(vg => vg.ReleaseDate)
                .HasColumnType("date")      // SQL Server date type
                .IsRequired();
             
            builder
                .Property(vg => vg.UserScore)
                .IsRequired();

            //Enforce UserScore range constraint at the database level
            builder.ToTable("VideoGame", table =>
            {
                table.HasCheckConstraint(
                    "CK_VideoGame_UserScore_Range",
                    "[UserScore] >= 0 AND [UserScore] <= 100");
            });

            // Relationships
            builder.HasMany(vg => vg.Genres)
                   .WithMany(g => g.VideoGames)
                   .UsingEntity<Dictionary<string, object>>(
                        "VideoGameGenre",
                        j => j.HasOne<Genre>()
                              .WithMany()
                              .HasForeignKey("GenreId")
                              .HasConstraintName("FK_VideoGameGenre_Genre_GenreId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<VideoGame>()
                              .WithMany()
                              .HasForeignKey("VideoGameId")
                              .HasConstraintName("FK_VideoGameGenre_VideoGame_VideoGameId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.ToTable("VideoGameGenre");
                            j.HasKey("VideoGameId", "GenreId");
                        });

            builder.HasMany(vg => vg.Platforms)
                   .WithMany(p => p.VideoGames)
                   .UsingEntity<Dictionary<string, object>>(
                        "VideoGamePlatform",
                        j => j.HasOne<Platform>()
                              .WithMany()
                              .HasForeignKey("PlatformId")
                              .HasConstraintName("FK_VideoGamePlatform_Platform_PlatformId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<VideoGame>()
                              .WithMany()
                              .HasForeignKey("VideoGameId")
                              .HasConstraintName("FK_VideoGamePlatform_VideoGame_VideoGameId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.ToTable("VideoGamePlatform");
                            j.HasKey("VideoGameId", "PlatformId");
                        });

            builder
                .HasOne(v => v.Publisher)
                .WithMany(c => c.PublishedGames)
                .HasForeignKey(v => v.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(v => v.Developer)
                .WithMany(c => c.DevelopedGames)
                .HasForeignKey(v => v.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}