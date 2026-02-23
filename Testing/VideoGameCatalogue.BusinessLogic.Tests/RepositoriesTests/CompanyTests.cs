using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;

namespace VideoGameCatalogue.BusinessLogic.Tests.RepositoriesTests
{
    public class CompanyTests
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
        public async Task GetAllAsync_WithNoCompanies_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleCompanies_ReturnsAllCompanies()
        {
            // Arrange
            using var context = CreateDbContext();
            var companies = new[]
            {
                new Company { Name = "Sony Interactive Entertainment" },
                new Company { Name = "Microsoft Game Studios" },
                new Company { Name = "Nintendo" }
            };
            context.Companies.AddRange(companies);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_DoesNotReturnDeletedCompanies()
        {
            // Arrange
            using var context = CreateDbContext();
            var company1 = new Company { Name = "Active Company" };
            var company2 = new Company { Name = "Deleted Company", isDeleted = true, DeletedOnDts = DateTime.UtcNow };

            context.Companies.AddRange(company1, company2);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Company", result.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNoTrackedEntities()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Test Company" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllAsync();
            var firstCompany = result.First();

            // Assert
            var entry = context.Entry(firstCompany);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        #endregion

        #region GetAllWithTrackingAsync Tests

        [Fact]
        public async Task GetAllWithTrackingAsync_WithNoCompanies_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllWithTrackingAsync_WithMultipleCompanies_ReturnsAllCompanies()
        {
            // Arrange
            using var context = CreateDbContext();
            var companies = new[]
            {
                new Company { Name = "Ubisoft" },
                new Company { Name = "EA Sports" }
            };
            context.Companies.AddRange(companies);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllWithTrackingAsync_ReturnsTrackedEntities()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Tracked Company" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();
            var firstCompany = result.First();

            // Assert
            var entry = context.Entry(firstCompany);
            Assert.Equal(EntityState.Unchanged, entry.State);
        }

        [Fact]
        public async Task GetAllWithTrackingAsync_DoesNotReturnDeletedCompanies()
        {
            // Arrange
            using var context = CreateDbContext();
            var company1 = new Company { Name = "Active Company" };
            var company2 = new Company { Name = "Deleted Company", isDeleted = true };
            context.Companies.AddRange(company1, company2);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllWithTrackingAsync();

            // Assert
            Assert.Single(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCompany()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Rockstar Games" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetByIdAsync(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(companyId, result.Id);
            Assert.Equal("Rockstar Games", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithDeletedCompanyId_ReturnsNull()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Deleted Company", isDeleted = true };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetByIdAsync(companyId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddWithReturningEntityAsync Tests

        [Fact]
        public async Task AddWithReturningEntityAsync_WithValidCompany_ReturnsCompanyWithId()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);
            var newCompany = new Company { Name = "New Company" };

            // Act
            var result = await repository.AddWithReturningEntityAsync(newCompany);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("New Company", result.Name);
        }

        [Fact]
        public async Task AddWithReturningEntityAsync_SavesCompanyToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);
            var newCompany = new Company { Name = "Persisted Company" };

            // Act
            var result = await repository.AddWithReturningEntityAsync(newCompany);

            // Assert
            Assert.NotNull(result);

            using var newContext = CreateDbContext();
            var savedCompany = await newContext.Companies
                .FirstOrDefaultAsync(c => c.Id == result.Id);

            Assert.NotNull(savedCompany);
            Assert.Equal("Persisted Company", savedCompany.Name);
        }

        [Fact]
        public async Task AddWithReturningEntityAsync_WithoutTracking_PreservesId()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);
            var newCompany = new Company { Name = "ID Test Company" };

            // Act
            var result = await repository.AddWithReturningEntityAsync(newCompany);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidCompany_UpdatesSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Original Name" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);
            company.Name = "Updated Name";

            // Act
            var result = await repository.UpdateAsync(company);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var updatedCompany = await newContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
            Assert.NotNull(updatedCompany);
            Assert.Equal("Updated Name", updatedCompany.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithNonexistentCompany_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);
            var nonexistentCompany = new Company { Id = 9999, Name = "Nonexistent" };

            // Act
            var result = await repository.UpdateAsync(nonexistentCompany);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_PreservesSoftDeleteState()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Deleted Company", isDeleted = true, DeletedOnDts = DateTime.UtcNow };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;
            var originalDeletedOn = company.DeletedOnDts;

            var repository = new CompanyRepository(context);
            var updatedCompany = new Company { Id = companyId, Name = "Updated Deleted Company" };

            // Act
            await repository.UpdateAsync(updatedCompany);

            // Assert
            using var newContext = CreateDbContext();
            var persistedCompany = await newContext.Companies
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.NotNull(persistedCompany);
            Assert.True(persistedCompany.isDeleted);
            Assert.Equal(originalDeletedOn, persistedCompany.DeletedOnDts);
        }

        #endregion

        #region DeleteAsync (Soft Delete) Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_SoftDeletesCompany()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "To Be Deleted" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.DeleteAsync(companyId);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var deletedCompany = await newContext.Companies
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.NotNull(deletedCompany);
            Assert.True(deletedCompany.isDeleted);
            Assert.NotNull(deletedCompany.DeletedOnDts);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.DeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithAlreadyDeletedCompany_ReturnsTrueButDoesNotReupdate()
        {
            // Arrange
            using var context = CreateDbContext();
            var originalTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var company = new Company { Name = "Already Deleted", isDeleted = true, DeletedOnDts = originalTime };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.DeleteAsync(companyId);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var deletedCompany = await newContext.Companies
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.NotNull(deletedCompany);
            Assert.Equal(originalTime, deletedCompany.DeletedOnDts);
        }

        #endregion

        #region RestoreAsync Tests

        [Fact]
        public async Task RestoreAsync_WithDeletedCompany_RestoresSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Deleted Company", isDeleted = true, DeletedOnDts = DateTime.UtcNow };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.RestoreAsync(companyId);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var restoredCompany = await newContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.NotNull(restoredCompany);
            Assert.False(restoredCompany.isDeleted);
            Assert.Null(restoredCompany.DeletedOnDts);
        }

        [Fact]
        public async Task RestoreAsync_WithNonexistentCompany_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.RestoreAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RestoreAsync_WithActiveCompany_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Active Company", isDeleted = false };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.RestoreAsync(companyId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region FullDeleteAsync Tests

        [Fact]
        public async Task FullDeleteAsync_WithValidId_PermanentlyDeletesCompany()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "To Be Fully Deleted" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.FullDeleteAsync(companyId);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var deletedCompany = await newContext.Companies
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.Null(deletedCompany);
        }

        [Fact]
        public async Task FullDeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.FullDeleteAsync(9999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task FullDeleteAsync_WithDeletedCompany_RemovesFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Soft Deleted Company", isDeleted = true };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            var companyId = company.Id;

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.FullDeleteAsync(companyId);

            // Assert
            Assert.True(result);
            using var newContext = CreateDbContext();
            var deletedCompany = await newContext.Companies
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            Assert.Null(deletedCompany);
        }

        #endregion

        #region GetAllIncludingDeletedAsync Tests

        [Fact]
        public async Task GetAllIncludingDeletedAsync_ReturnsActiveAndDeletedCompanies()
        {
            // Arrange
            using var context = CreateDbContext();
            var company1 = new Company { Name = "Active Company" };
            var company2 = new Company { Name = "Deleted Company", isDeleted = true };
            context.Companies.AddRange(company1, company2);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllIncludingDeletedAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Active Company" && !c.isDeleted);
            Assert.Contains(result, c => c.Name == "Deleted Company" && c.isDeleted);
        }

        [Fact]
        public async Task GetAllIncludingDeletedAsync_WithNoCompanies_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new CompanyRepository(context);

            // Act
            var result = await repository.GetAllIncludingDeletedAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        

        #region CancellationToken Tests

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            var company = new Company { Name = "Test Company" };
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            var repository = new CompanyRepository(context);
            var cts = new CancellationTokenSource();

            // Act
            var result = await repository.GetAllAsync(cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        #endregion
    }
}