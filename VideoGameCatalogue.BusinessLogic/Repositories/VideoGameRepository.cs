using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface IVideoGameRepository : IRepositoryBase<VideoGame>
    {
        Task<VideoGame> AddWithGenresAsync(VideoGame entity, IEnumerable<int> genreIds, CancellationToken token = default);
        Task<VideoGame?> UpdateWithGenresAsync(VideoGame entity, IEnumerable<int> genreIds, CancellationToken token = default);
    }

    public class VideoGameRepository : RepositoryBase<VideoGame>, IVideoGameRepository
    {
        public VideoGameRepository(VideoGameCatalogueContext dbContext) : base(dbContext) { }

        public override async Task<IEnumerable<VideoGame>> GetAllAsync(CancellationToken token = default)
        {
            return await _dbSet
                .Include(v => v.Genres)
                .AsNoTracking()
                .ToListAsync(token);
        }

        public override async Task<VideoGame?> GetByIdAsync(int id, CancellationToken token = default)
        {
            return await _dbSet
                .Include(v => v.Genres)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id, token);
        }

        public override async Task<IEnumerable<VideoGame>> GetAllIncludingDeletedAsync(CancellationToken token = default)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(v => v.Genres)
                .AsNoTracking()
                .ToListAsync(token);
        }

        public async Task<VideoGame> AddWithGenresAsync(VideoGame entity, IEnumerable<int> genreIds, CancellationToken token = default)
        {
            var ids = (genreIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (ids.Count == 0)
                throw new InvalidOperationException("At least one GenreId is required.");

            // Validate IDs exist
            var existingIds = await _context.Set<Genre>()
                .Where(g => ids.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync(token);

            var missing = ids.Except(existingIds).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"Invalid GenreIds: {string.Join(", ", missing)}");

            // Attach stubs 
            entity.Genres = new List<Genre>();
            foreach (var id in ids)
            {
                var stub = new Genre { Id = id };
                _context.Attach(stub);
                entity.Genres.Add(stub);
            }

            await _dbSet.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);

            // Reload to populate teh full Genre
            var reloaded = await _dbSet
                .Include(v => v.Genres)
                .AsNoTracking()
                .FirstAsync(v => v.Id == entity.Id, token);

            return reloaded;
        }

        public async Task<VideoGame?> UpdateWithGenresAsync(
    VideoGame entity,
    IEnumerable<int> genreIds,
    CancellationToken token = default)
        {
            // 1) tracked entity
            var existing = await _dbSet
                .Include(v => v.Genres)
                .FirstOrDefaultAsync(v => v.Id == entity.Id, token);

            if (existing == null)
                return null;

            // 2) update scalars on the tracked entity
            existing.Title = entity.Title;
            existing.Synopsis = entity.Synopsis;
            existing.ReleaseDate = entity.ReleaseDate;
            existing.UserScore = entity.UserScore;

            // 3) validate + load REAL Genre entities (not stubs)
            var ids = (genreIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (ids.Count == 0)
                throw new InvalidOperationException("At least one GenreId is required.");

            var genres = await _context.Set<Genre>()
                .Where(g => ids.Contains(g.Id))
                .ToListAsync(token);

            var missing = ids.Except(genres.Select(g => g.Id)).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"Invalid GenreIds: {string.Join(", ", missing)}");

            // 4) replace many-to-many relationships
            existing.Genres.Clear();
            foreach (var g in genres)
                existing.Genres.Add(g);

            // 5) force EF to see changes (harmless if already enabled)
            _context.ChangeTracker.DetectChanges();

            await _context.SaveChangesAsync(token);

            // 6) reload for response
            return await _dbSet
                .Include(v => v.Genres)
                .AsNoTracking()
                .FirstAsync(v => v.Id == existing.Id, token);
        }
    }
}