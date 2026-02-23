using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface IVideoGameRepository : IRepositoryBase<VideoGame>
    {
         
        Task<VideoGame> AddWithRelationshipsAsync(
            VideoGame entity,
            IEnumerable<int> genreIds,
            IEnumerable<int> platformIds,
            int? publisherId,
            int? developerId,
            byte[]? coverImageBytes,
            string? coverImageContentType,
            CancellationToken token = default);

        Task<VideoGame?> UpdateWithRelationshipsAsync(
            VideoGame entity,
            IEnumerable<int> genreIds,
            IEnumerable<int> platformIds,
            int? publisherId,
            int? developerId,
            byte[]? coverImageBytes,
            string? coverImageContentType,
            bool overwriteCoverImage,
            CancellationToken token = default);
    }

    public class VideoGameRepository : RepositoryBase<VideoGame>, IVideoGameRepository
    {
        public VideoGameRepository(VideoGameCatalogueContext dbContext) : base(dbContext) { }

        public override async Task<IEnumerable<VideoGame>> GetAllAsync(CancellationToken token = default)
        {
            return await _dbSet
                .Where(v => !v.isDeleted)
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .Include(v => v.Publisher)
                .Include(v => v.Developer)
                .AsNoTracking()
                .ToListAsync(token);
        }


        public override async Task<VideoGame?> GetByIdAsync(int id, CancellationToken token = default)
        {
            return await _dbSet
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .Include(v => v.Publisher)
                .Include(v => v.Developer)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id, token);
        }

        public override async Task<IEnumerable<VideoGame>> GetAllIncludingDeletedAsync(CancellationToken token = default)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .Include(v => v.Publisher)
                .Include(v => v.Developer)
                .AsNoTracking()
                .ToListAsync(token);
        }
        public async Task<VideoGame> AddWithRelationshipsAsync(
            VideoGame entity,
            IEnumerable<int> genreIds,
            IEnumerable<int> platformIds,
            int? publisherId,
            int? developerId,
            byte[]? coverImageBytes,
            string? coverImageContentType,
            CancellationToken token = default)
        {
            // --- Genres ---
            var gIds = (genreIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (gIds.Count == 0)
                throw new InvalidOperationException("At least one GenreId is required.");

            var existingGenreIds = await _context.Set<Genre>()
                .Where(g => gIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync(token);

            var missingGenres = gIds.Except(existingGenreIds).ToList();
            if (missingGenres.Count > 0)
                throw new InvalidOperationException($"Invalid GenreIds: {string.Join(", ", missingGenres)}");

            // --- Platforms ---
            var pIds = (platformIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (pIds.Count == 0)
                throw new InvalidOperationException("At least one PlatformId is required.");

            var existingPlatformIds = await _context.Set<Platform>()
                .Where(p => pIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(token);

            var missingPlatforms = pIds.Except(existingPlatformIds).ToList();
            if (missingPlatforms.Count > 0)
                throw new InvalidOperationException($"Invalid PlatformIds: {string.Join(", ", missingPlatforms)}");

            // --- Publisher / Developer  
            if (publisherId.HasValue)
            {
                var pubExists = await _context.Set<Company>()
                    .AnyAsync(c => c.Id == publisherId.Value, token);

                if (!pubExists)
                    throw new InvalidOperationException($"Invalid PublisherId: {publisherId.Value}");
            }

            if (developerId.HasValue)
            {
                var devExists = await _context.Set<Company>()
                    .AnyAsync(c => c.Id == developerId.Value, token);

                if (!devExists)
                    throw new InvalidOperationException($"Invalid DeveloperId: {developerId.Value}");
            }

            // --- Apply scalar relationship fields / cover ---
            entity.PublisherId = publisherId;
            entity.DeveloperId = developerId;
            entity.CoverImageBytes = coverImageBytes;
            entity.CoverImageContentType = coverImageContentType;

            // --- Attach relationships
            // Fetch actual tracked Genre entities to avoid duplicate key conflicts
            var genres = await _context.Set<Genre>()
                .Where(g => gIds.Contains(g.Id))
                .ToListAsync(token);
            entity.Genres = genres;

            // Fetch actual tracked Platform entities to avoid duplicate key conflicts
            var platforms = await _context.Set<Platform>()
                .Where(p => pIds.Contains(p.Id))
                .ToListAsync(token);
            entity.Platforms = platforms;

            await _dbSet.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);

            // Reload for response
           return await _dbSet
               .IgnoreQueryFilters()
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .Include(v => v.Publisher)
                .Include(v => v.Developer)
                .AsNoTracking()
                .FirstAsync(v => v.Id == entity.Id, token);
        }
        public async Task<VideoGame?> UpdateWithRelationshipsAsync(
            VideoGame entity,
            IEnumerable<int> genreIds,
            IEnumerable<int> platformIds,
            int? publisherId,
            int? developerId,
            byte[]? coverImageBytes,
            string? coverImageContentType,
            bool overwriteCoverImage,
            CancellationToken token = default)
        {
            // 1) tracked entity + navs
            var existing = await _dbSet
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .FirstOrDefaultAsync(v => v.Id == entity.Id, token);

            if (existing == null)
                return null;

            // 2) update scalars
            existing.Title = entity.Title;
            existing.Synopsis = entity.Synopsis;
            existing.ReleaseDate = entity.ReleaseDate;
            existing.UserScore = entity.UserScore;

            existing.PublisherId = publisherId;
            existing.DeveloperId = developerId;

            // Cover image: only replace if requested
            if (overwriteCoverImage)
            {
                existing.CoverImageBytes = coverImageBytes;
                existing.CoverImageContentType = coverImageContentType;
            }

            // 3) validate + load Genres
            var gIds = (genreIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (gIds.Count == 0)
                throw new InvalidOperationException("At least one GenreId is required.");

            var genres = await _context.Set<Genre>()
                .Where(g => gIds.Contains(g.Id))
                .ToListAsync(token);

            var missingGenres = gIds.Except(genres.Select(g => g.Id)).ToList();
            if (missingGenres.Count > 0)
                throw new InvalidOperationException($"Invalid GenreIds: {string.Join(", ", missingGenres)}");

            // 4) validate + load Platforms
            var pIds = (platformIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (pIds.Count == 0)
                throw new InvalidOperationException("At least one PlatformId is required.");

            var platforms = await _context.Set<Platform>()
                .Where(p => pIds.Contains(p.Id))
                .ToListAsync(token);

            var missingPlatforms = pIds.Except(platforms.Select(p => p.Id)).ToList();
            if (missingPlatforms.Count > 0)
                throw new InvalidOperationException($"Invalid PlatformIds: {string.Join(", ", missingPlatforms)}");

            // 5) validate Publisher/Developer existence 
            if (publisherId.HasValue)
            {
                var pubExists = await _context.Set<Company>()
                    .AnyAsync(c => c.Id == publisherId.Value, token);

                if (!pubExists)
                    throw new InvalidOperationException($"Invalid PublisherId: {publisherId.Value}");
            }

            if (developerId.HasValue)
            {
                var devExists = await _context.Set<Company>()
                    .AnyAsync(c => c.Id == developerId.Value, token);

                if (!devExists)
                    throw new InvalidOperationException($"Invalid DeveloperId: {developerId.Value}");
            }

            // 6) replace many-to-many relationships
            existing.Genres.Clear();
            foreach (var g in genres)
                existing.Genres.Add(g);

            existing.Platforms.Clear();
            foreach (var p in platforms)
                existing.Platforms.Add(p);

            // 7) ensure EF sees changes
            _context.ChangeTracker.DetectChanges();

            await _context.SaveChangesAsync(token);

            // 8) reload for response
            return await _dbSet
                .Include(v => v.Genres)
                .Include(v => v.Platforms)
                .Include(v => v.Publisher)
                .Include(v => v.Developer)
                .AsNoTracking()
                .FirstAsync(v => v.Id == existing.Id, token);
        }
        

    }
}