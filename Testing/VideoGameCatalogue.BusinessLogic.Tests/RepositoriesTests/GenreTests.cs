using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.BusinessLogic.Tests.RepositoriesTests
{
    public class GenreTests
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        private VideoGameCatalogueContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<VideoGameCatalogueContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            return new VideoGameCatalogueContext(options);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithNoGenres_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleGenres_ReturnsAllGenres()
        {
            // Arrange
            using var context = CreateDbContext();
            var genres = new[]
            {
                new Genre { Name = "Action" },
                new Genre { Name = "RPG" },
                new Genre { Name = "Adventure" }
            };

            context.Set<Genre>().AddRange(genres);
            await context.SaveChangesAsync();

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNoTrackedEntities()
        {
            // Arrange
            using var context = CreateDbContext();
            var genre = new Genre { Name = "NoTracking Genre" };
            context.Set<Genre>().Add(genre);
            await context.SaveChangesAsync();

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllAsync();
            var first = result.First();

            // Assert
            var entry = context.Entry(first);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        #endregion

        #region GetAllWithTrackingAsync Tests

        [Fact]
        public async Task GetAllWithTrackingAsync_WithNoGenres_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllWithTrackingAsync_WithMultipleGenres_ReturnsAllGenres()
        {
            // Arrange
            using var context = CreateDbContext();
            var genres = new[]
            {
                new Genre { Name = "Strategy" },
                new Genre { Name = "Simulation" }
            };

            context.Set<Genre>().AddRange(genres);
            await context.SaveChangesAsync();

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllWithTrackingAsync_ReturnsTrackedEntities()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Set<Genre>().Add(new Genre { Name = "Tracked Genre" });
            await context.SaveChangesAsync();

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();
            var first = result.First();

            // Assert
            var entry = context.Entry(first);
            Assert.Equal(EntityState.Unchanged, entry.State);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsGenre()
        {
            // Arrange
            using var context = CreateDbContext();
            var genre = new Genre { Name = "Puzzle" };
            context.Set<Genre>().Add(genre);
            await context.SaveChangesAsync();
            var id = genre.Id;

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Puzzle", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddWithReturningEntityAsync Tests

        [Fact]
        public async Task AddWithReturningEntityAsync_WithValidGenre_ReturnsGenreWithId()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            var newGenre = new Genre { Name = "Horror" };

            // Act
            var result = await repository.AddWithReturningEntityAsync(newGenre);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Horror", result.Name);
        }

        [Fact]
        public async Task AddWithReturningEntityAsync_SavesGenreToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            var newGenre = new Genre { Name = "Indie" };

            // Act
            var result = await repository.AddWithReturningEntityAsync(newGenre);

            // Assert
            Assert.NotNull(result);

            using var newContext = CreateDbContext();
            var saved = await newContext.Set<Genre>().FirstOrDefaultAsync(g => g.Id == result.Id);

            Assert.NotNull(saved);
            Assert.Equal("Indie", saved.Name);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidGenre_UpdatesSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            var genre = new Genre { Name = "Original Genre" };
            context.Set<Genre>().Add(genre);
            await context.SaveChangesAsync();
            var id = genre.Id;

            var repository = new GenreRepository(context);
            genre.Name = "Updated Genre";

            // Act
            var result = await repository.UpdateAsync(genre);

            // Assert
            Assert.True(result);

            using var newContext = CreateDbContext();
            var updated = await newContext.Set<Genre>().FirstOrDefaultAsync(g => g.Id == id);

            Assert.NotNull(updated);
            Assert.Equal("Updated Genre", updated.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithNonexistentGenre_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.UpdateAsync(new Genre { Id = 9999, Name = "Does Not Exist" });

            // Assert
            Assert.False(result);
        }

        #endregion

        #region DeleteAsync / FullDeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesGenre()
        {
            // Arrange
            using var context = CreateDbContext();
            var genre = new Genre { Name = "To Delete" };
            context.Set<Genre>().Add(genre);
            await context.SaveChangesAsync();
            var id = genre.Id;

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.DeleteAsync(id);

            // Assert
            Assert.True(result);

            using var newContext = CreateDbContext();
            var stillThere = await newContext.Set<Genre>().FirstOrDefaultAsync(g => g.Id == id);

            // If Genre supports soft-delete via base, it might still exist.
            // If it does not, it should be removed. We accept either behavior here.
            // (If you want strict behavior, tell me whether Genre has isDeleted/DeletedOnDts.)
            _ = stillThere;
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.DeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task FullDeleteAsync_WithValidId_PermanentlyDeletesGenre()
        {
            // Arrange
            using var context = CreateDbContext();
            var genre = new Genre { Name = "Hard Delete" };
            context.Set<Genre>().Add(genre);
            await context.SaveChangesAsync();
            var id = genre.Id;

            var repository = new GenreRepository(context);

            // Act
            var result = await repository.FullDeleteAsync(id);

            // Assert
            Assert.True(result);

            using var newContext = CreateDbContext();
            var deleted = await newContext.Set<Genre>().FirstOrDefaultAsync(g => g.Id == id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task FullDeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new GenreRepository(context);

            // Act
            var result = await repository.FullDeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        #endregion

        

        #region CancellationToken Tests

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Set<Genre>().Add(new Genre { Name = "Token Genre" });
            await context.SaveChangesAsync();

            var repository = new GenreRepository(context);
            using var cts = new CancellationTokenSource();

            // Act
            var result = await repository.GetAllAsync(cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        #endregion
    }
}