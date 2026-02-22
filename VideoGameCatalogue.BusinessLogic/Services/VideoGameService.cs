using System;
using System.Threading;
using System.Threading.Tasks;
using VideoGameCatalogue.BusinessLogic.Repositories;
using VideoGameCatalogue.Data.Models.Contracts.Requests;
using VideoGameCatalogue.Data.Models.Entities;
using VideoGameCatalogue.Data.Models.Mapping;
using VideoGameCatalogue.Shared.Base;

public interface IVideoGameService : IServiceBase<VideoGame>
{
    Task<VideoGame> AddWithRelationshipsAsync(CreateVideoGameRequest request, CancellationToken token = default);

    Task<VideoGame?> UpdateWithRelationshipsAsync(UpdateVideoGameRequest request, CancellationToken token = default);
}

public class VideoGameService : ServiceBase<VideoGame>, IVideoGameService
{
    private readonly IVideoGameRepository _repo;

    public VideoGameService(IVideoGameRepository repo) : base(repo)
    {
        _repo = repo;
    }

    public Task<VideoGame> AddWithRelationshipsAsync(CreateVideoGameRequest request, CancellationToken token = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        byte[]? coverBytes = null;
        if (!string.IsNullOrWhiteSpace(request.CoverImageBase64))
            coverBytes = Convert.FromBase64String(request.CoverImageBase64);

        return _repo.AddWithRelationshipsAsync(
            request.MapToEntity(),
            request.GenreIds,
            request.PlatformIds,
            request.PublisherId,
            request.DeveloperId,
            coverBytes,
            request.CoverImageContentType,
            token);
    }

    public Task<VideoGame?> UpdateWithRelationshipsAsync(UpdateVideoGameRequest request, CancellationToken token = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var overwriteCover = !string.IsNullOrWhiteSpace(request.CoverImageBase64);

        byte[]? coverBytes = null; 

        if (!string.IsNullOrWhiteSpace(request.CoverImageBase64))
        {
            coverBytes = Convert.FromBase64String(request.CoverImageBase64);
            overwriteCover = true;
        }

        return _repo.UpdateWithRelationshipsAsync(
            request.MapToEntity(request.Id),
            request.GenreIds,
            request.PlatformIds,
            request.PublisherId,
            request.DeveloperId,
            coverBytes,
            request.CoverImageContentType,
            overwriteCover,
            token);
    }
     
}