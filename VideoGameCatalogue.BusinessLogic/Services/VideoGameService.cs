using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Data.Models.Mapping;
using VideoGameCatalogue.Shared.Base;

public interface IVideoGameService : IServiceBase<VideoGame>
{
    Task<VideoGame> AddWithGenresAsync(CreateVideoGameRequest request, CancellationToken token = default);
    Task<VideoGame?> UpdateWithGenresAsync(UpdateVideoGameRequest request, CancellationToken token = default);
}

    public class VideoGameService : ServiceBase<VideoGame>, IVideoGameService
    {
        private readonly IVideoGameRepository _repo;

        public VideoGameService(IVideoGameRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public Task<VideoGame> AddWithGenresAsync(CreateVideoGameRequest request, CancellationToken token = default)
            => _repo.AddWithGenresAsync(request.MapToEntity(), request.GenreIds, token);

        public Task<VideoGame?> UpdateWithGenresAsync(UpdateVideoGameRequest request, CancellationToken token = default)
            => _repo.UpdateWithGenresAsync(request.MapToEntity(request.Id), request.GenreIds, token);
    }
