using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Shared.Base
{
    public interface IServiceBase<T> where T : EntityBase
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);
        Task<IEnumerable<T>> GetAllWithTrackingAsync(CancellationToken token = default);
        Task<T?> GetByIdAsync(int id, CancellationToken token = default);
        Task<T?> AddWithReturningEntityAsync(T entity, CancellationToken token = default);
        Task<bool> UpdateAsync(T entity, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<bool> RestoreAsync(int id, CancellationToken token = default);
        Task<bool> FullDeleteAsync(int id, CancellationToken token = default);
        Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken token = default);
        Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken token = default);
        Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken token = default);
    }

    public class ServiceBase<T> : IServiceBase<T> where T : EntityBase
    {
        protected readonly IRepositoryBase<T> _repository;

        public ServiceBase(IRepositoryBase<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default) => await _repository.GetAllAsync(token);

        public async Task<IEnumerable<T>> GetAllWithTrackingAsync(CancellationToken token = default) => await _repository.GetAllWithTrackingAsync(token);

        public async Task<T?> GetByIdAsync(int id, CancellationToken token = default)    => await _repository.GetByIdAsync(id, token);

        public async Task<T?> AddWithReturningEntityAsync(T entity, CancellationToken token = default) => await _repository.AddWithReturningEntityAsync(entity, token);
        public async Task<bool> UpdateAsync(T entity, CancellationToken token = default) => await _repository.UpdateAsync(entity, token);

        public async Task<bool> DeleteAsync(int id, CancellationToken token = default) => await _repository.DeleteAsync(id, token);

        public async Task<bool> RestoreAsync(int id, CancellationToken token = default) => await _repository.RestoreAsync(id, token);

        public async Task<bool> FullDeleteAsync(int id, CancellationToken token = default) => await _repository.FullDeleteAsync(id, token);

        public async Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken token = default) => await _repository.GetAllIncludingDeletedAsync(token);
        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken token = default) =>  await _repository.AddRangeAsync(entities, token);
        
        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken token = default) => await  _repository.DeleteRangeAsync(ids, token);
        
    }
}
