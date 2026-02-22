using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data.Data;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Shared.Base;

namespace VideoGameCatalogue.BusinessLogic.Repositories
{
    public interface IVideoGameRepository : IRepositoryBase<VideoGame>
    {

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
    }
}