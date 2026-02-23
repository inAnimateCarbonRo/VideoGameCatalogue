using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.BusinessLogic.Tests.RepositoriesTests
{
    public class VideoGameTests
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        private VideoGameCatalogueContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<VideoGameCatalogueContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            return new VideoGameCatalogueContext(options);
        }

        private async Task<(int GenreId, int PlatformId)> SeedGenreAndPlatformAsync(VideoGameCatalogueContext context)
        {
            var genre = new Genre { Name = "Action" };
            var platform = new Platform { Name = "PC" };

            context.Set<Genre>().Add(genre);
            context.Set<Platform>().Add(platform);

            await context.SaveChangesAsync();

            return (genre.Id, platform.Id);
        }

        private async Task<(int PublisherId, int DeveloperId)> SeedPublisherAndDeveloperAsync(VideoGameCatalogueContext context)
        {
            var publisher = new Company { Name = "Publisher Co" };
            var developer = new Company { Name = "Developer Co" };

            context.Companies.AddRange(publisher, developer);
            await context.SaveChangesAsync();

            return (publisher.Id, developer.Id);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithNoVideoGames_ReturnsEmptyList()
        {
            using var context = CreateDbContext();
            var repository = new VideoGameRepository(context);

            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_DoesNotReturnDeletedVideoGames()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            // Add one active and one deleted via the repository (so relationships are valid)
            var repo = new VideoGameRepository(context);

            var active = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Active Game",
                    Synopsis = "Active synopsis",
                    ReleaseDate = new DateOnly(2020, 1, 1),
                    UserScore = 8
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            // Soft-delete directly on the entity for this test (matches repo filter: !v.isDeleted)
            var deleted = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Deleted Game",
                    Synopsis = "Deleted synopsis",
                    ReleaseDate = new DateOnly(2019, 1, 1),
                    UserScore = 7,
                    isDeleted = true,
                    DeletedOnDts = DateTime.UtcNow
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);

            var only = result.First();
            Assert.Equal(active.Id, only.Id);
            Assert.Equal("Active Game", only.Title);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNoTrackedEntities()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);
            await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Tracked Check",
                    Synopsis = "Synopsis",
                    ReleaseDate = new DateOnly(2021, 1, 1),
                    UserScore = 9
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            var result = await repo.GetAllAsync();
            var first = result.First();

            var entry = context.Entry(first);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsGameWithRelationships()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);
            var (publisherId, developerId) = await SeedPublisherAndDeveloperAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Relationship Game",
                    Synopsis = "Has relations",
                    ReleaseDate = new DateOnly(2022, 5, 5),
                    UserScore = 8
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: publisherId,
                developerId: developerId,
                coverImageBytes: new byte[] { 1, 2, 3 },
                coverImageContentType: "image/png");

            var result = await repo.GetByIdAsync(created.Id);

            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal("Relationship Game", result.Title);

            Assert.NotNull(result.Genres);
            Assert.Single(result.Genres);
            Assert.Equal(genreId, result.Genres.First().Id);

            Assert.NotNull(result.Platforms);
            Assert.Single(result.Platforms);
            Assert.Equal(platformId, result.Platforms.First().Id);

            Assert.NotNull(result.Publisher);
            Assert.Equal(publisherId, result.Publisher.Id);

            Assert.NotNull(result.Developer);
            Assert.Equal(developerId, result.Developer.Id);

            Assert.NotNull(result.CoverImageBytes);
            Assert.Equal("image/png", result.CoverImageContentType);

            // AsNoTracking
            var entry = context.Entry(result);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            using var context = CreateDbContext();
            var repo = new VideoGameRepository(context);

            var result = await repo.GetByIdAsync(9999);

            Assert.Null(result);
        }

        #endregion

        #region GetAllIncludingDeletedAsync Tests

        [Fact]
        public async Task GetAllIncludingDeletedAsync_ReturnsActiveAndDeleted()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Active",
                    Synopsis = "A",
                    ReleaseDate = new DateOnly(2020, 1, 1),
                    UserScore = 8
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Deleted",
                    Synopsis = "D",
                    ReleaseDate = new DateOnly(2018, 1, 1),
                    UserScore = 60,
                    isDeleted = true,
                    DeletedOnDts = DateTime.UtcNow
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            var result = await repo.GetAllIncludingDeletedAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            Assert.Contains(result, v => v.Title == "Active" && !v.isDeleted);
            Assert.Contains(result, v => v.Title == "Deleted" && v.isDeleted);
        }

        [Fact]
        public async Task GetAllIncludingDeletedAsync_WithNoVideoGames_ReturnsEmptyList()
        {
            using var context = CreateDbContext();
            var repo = new VideoGameRepository(context);

            var result = await repo.GetAllIncludingDeletedAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region AddWithRelationshipsAsync Tests

        [Fact]
        public async Task AddWithRelationshipsAsync_WithValidData_CreatesGameWithRelationships()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);
            var (publisherId, developerId) = await SeedPublisherAndDeveloperAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "New Game",
                    Synopsis = "Cool",
                    ReleaseDate = new DateOnly(2023, 3, 3),
                    UserScore = 9
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: publisherId,
                developerId: developerId,
                coverImageBytes: new byte[] { 10, 11 },
                coverImageContentType: "image/jpeg");

            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("New Game", created.Title);

            Assert.NotNull(created.Genres);
            Assert.Single(created.Genres);

            Assert.NotNull(created.Platforms);
            Assert.Single(created.Platforms);

            Assert.NotNull(created.Publisher);
            Assert.NotNull(created.Developer);

            Assert.NotNull(created.CoverImageBytes);
            Assert.Equal("image/jpeg", created.CoverImageContentType);
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithEmptyGenreIds_Throws()
        {
            using var context = CreateDbContext();
            var (_, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: Array.Empty<int>(),
                    platformIds: new[] { platformId },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithEmptyPlatformIds_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, _) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: new[] { genreId },
                    platformIds: Array.Empty<int>(),
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithInvalidGenreId_Throws()
        {
            using var context = CreateDbContext();
            var (_, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: new[] { 9999 },
                    platformIds: new[] { platformId },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithInvalidPlatformId_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, _) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: new[] { genreId },
                    platformIds: new[] { 9999 },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithInvalidPublisherId_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: new[] { genreId },
                    platformIds: new[] { platformId },
                    publisherId: 9999,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        [Fact]
        public async Task AddWithRelationshipsAsync_WithInvalidDeveloperId_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.AddWithRelationshipsAsync(
                    new VideoGame { Title = "Bad", Synopsis = "Bad", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                    genreIds: new[] { genreId },
                    platformIds: new[] { platformId },
                    publisherId: null,
                    developerId: 9999,
                    coverImageBytes: null,
                    coverImageContentType: null));
        }

        #endregion

        #region UpdateWithRelationshipsAsync Tests

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithNonexistentGame_ReturnsNull()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var result = await repo.UpdateWithRelationshipsAsync(
                new VideoGame
                {
                    Id = 9999,
                    Title = "Doesn't matter",
                    Synopsis = "N/A",
                    ReleaseDate = new DateOnly(2000, 1, 1),
                    UserScore = 0
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null,
                overwriteCoverImage: false);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithValidData_UpdatesScalarsAndRelationships()
        {
            using var context = CreateDbContext();

            // Seed initial related entities
            var (g1, p1) = await SeedGenreAndPlatformAsync(context);
            var (pubId, devId) = await SeedPublisherAndDeveloperAsync(context);

            // Add a second genre/platform for update
            var genre2 = new Genre { Name = "RPG" };
            var platform2 = new Platform { Name = "PS5" };
            context.Set<Genre>().Add(genre2);
            context.Set<Platform>().Add(platform2);
            await context.SaveChangesAsync();

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Original",
                    Synopsis = "Original synopsis",
                    ReleaseDate = new DateOnly(2020, 1, 1),
                    UserScore = 7
                },
                genreIds: new[] { g1 },
                platformIds: new[] { p1 },
                publisherId: pubId,
                developerId: devId,
                coverImageBytes: new byte[] { 1, 2, 3 },
                coverImageContentType: "image/png");

            // Update: change scalars and swap relationships
            var updated = await repo.UpdateWithRelationshipsAsync(
                new VideoGame
                {
                    Id = created.Id,
                    Title = "Updated",
                    Synopsis = "Updated synopsis",
                    ReleaseDate = new DateOnly(2021, 2, 2),
                    UserScore = 9
                },
                genreIds: new[] { genre2.Id },
                platformIds: new[] { platform2.Id },
                publisherId: pubId,
                developerId: devId,
                coverImageBytes: null,
                coverImageContentType: null,
                overwriteCoverImage: false);

            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal("Updated", updated.Title);
            Assert.Equal("Updated synopsis", updated.Synopsis);
            Assert.Equal(new DateOnly(2021, 2, 2), updated.ReleaseDate);
            Assert.Equal(9, updated.UserScore);

            Assert.NotNull(updated.Genres);
            Assert.Single(updated.Genres);
            Assert.Equal(genre2.Id, updated.Genres.First().Id);

            Assert.NotNull(updated.Platforms);
            Assert.Single(updated.Platforms);
            Assert.Equal(platform2.Id, updated.Platforms.First().Id);

            // Cover should be preserved since overwriteCoverImage == false
            Assert.NotNull(updated.CoverImageBytes);
            Assert.Equal("image/png", updated.CoverImageContentType);
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WhenOverwriteCoverImageTrue_ReplacesCoverFields()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Original",
                    Synopsis = "Original synopsis",
                    ReleaseDate = new DateOnly(2020, 1, 1),
                    UserScore = 7
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: new byte[] { 1, 2, 3 },
                coverImageContentType: "image/png");

            var newCover = new byte[] { 9, 9, 9 };

            var updated = await repo.UpdateWithRelationshipsAsync(
                new VideoGame
                {
                    Id = created.Id,
                    Title = "Original",
                    Synopsis = "Original synopsis",
                    ReleaseDate = new DateOnly(2020, 1, 1),
                    UserScore = 7
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: newCover,
                coverImageContentType: "image/jpeg",
                overwriteCoverImage: true);

            Assert.NotNull(updated);
            Assert.NotNull(updated.CoverImageBytes);
            Assert.Equal("image/jpeg", updated.CoverImageContentType);
            Assert.Equal(newCover, updated.CoverImageBytes);
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithEmptyGenreIds_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame { Title = "Game", Synopsis = "S", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.UpdateWithRelationshipsAsync(
                    new VideoGame { Id = created.Id, Title = "U", Synopsis = "U", ReleaseDate = new DateOnly(2021, 1, 1), UserScore = 2 },
                    genreIds: Array.Empty<int>(),
                    platformIds: new[] { platformId },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null,
                    overwriteCoverImage: false));
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithEmptyPlatformIds_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame { Title = "Game", Synopsis = "S", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.UpdateWithRelationshipsAsync(
                    new VideoGame { Id = created.Id, Title = "U", Synopsis = "U", ReleaseDate = new DateOnly(2021, 1, 1), UserScore = 2 },
                    genreIds: new[] { genreId },
                    platformIds: Array.Empty<int>(),
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null,
                    overwriteCoverImage: false));
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithInvalidGenreId_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame { Title = "Game", Synopsis = "S", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.UpdateWithRelationshipsAsync(
                    new VideoGame { Id = created.Id, Title = "U", Synopsis = "U", ReleaseDate = new DateOnly(2021, 1, 1), UserScore = 2 },
                    genreIds: new[] { 9999 },
                    platformIds: new[] { platformId },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null,
                    overwriteCoverImage: false));
        }

        [Fact]
        public async Task UpdateWithRelationshipsAsync_WithInvalidPlatformId_Throws()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            var created = await repo.AddWithRelationshipsAsync(
                new VideoGame { Title = "Game", Synopsis = "S", ReleaseDate = new DateOnly(2020, 1, 1), UserScore = 1 },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repo.UpdateWithRelationshipsAsync(
                    new VideoGame { Id = created.Id, Title = "U", Synopsis = "U", ReleaseDate = new DateOnly(2021, 1, 1), UserScore = 2 },
                    genreIds: new[] { genreId },
                    platformIds: new[] { 9999 },
                    publisherId: null,
                    developerId: null,
                    coverImageBytes: null,
                    coverImageContentType: null,
                    overwriteCoverImage: false));
        }

        #endregion

        #region CancellationToken Tests

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_CompletesSuccessfully()
        {
            using var context = CreateDbContext();
            var (genreId, platformId) = await SeedGenreAndPlatformAsync(context);

            var repo = new VideoGameRepository(context);

            await repo.AddWithRelationshipsAsync(
                new VideoGame
                {
                    Title = "Token Game",
                    Synopsis = "Token synopsis",
                    ReleaseDate = new DateOnly(2024, 1, 1),
                    UserScore = 8
                },
                genreIds: new[] { genreId },
                platformIds: new[] { platformId },
                publisherId: null,
                developerId: null,
                coverImageBytes: null,
                coverImageContentType: null);

            using var cts = new CancellationTokenSource();

            var result = await repo.GetAllAsync(cts.Token);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        #endregion
    }
}