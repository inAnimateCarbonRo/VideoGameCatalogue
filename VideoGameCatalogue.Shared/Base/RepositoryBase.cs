using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGameCatalogue.Shared.Base
{
    internal class RepositoryBase
    {
    }

    public interface IRepositoryBase<T> where T : EntityBase
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
        
    }
    public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.isDeleted)
                    .AsNoTracking()
                    .ToListAsync(token);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                var entityType = typeof(T);
                var requiredStringProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)));
                var propNames = string.Join(", ", requiredStringProps.Select(p => p.Name));
                throw new Exception($"Error loading entities of type '{entityType.Name}': {ex.Message}. Possible required string properties that may be null in the database: {propNames}. Check your entity and database schema for mismatches.", ex);
            }
            catch (Exception ex)
            {
                var entityType = typeof(T).Name;
                throw new Exception($"Error loading all entities of type '{entityType}': {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllWithTrackingAsync(CancellationToken token = default)
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.isDeleted)
                    .ToListAsync(token);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                var entityType = typeof(T);
                var requiredStringProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)));
                var propNames = string.Join(", ", requiredStringProps.Select(p => p.Name));
                throw new Exception($"Error loading entities of type '{entityType.Name}' (with tracking): {ex.Message}. Possible required string properties that may be null in the database: {propNames}. Check your entity and database schema for mismatches.", ex);
            }
            catch (Exception ex)
            {
                var entityType = typeof(T).Name;
                throw new Exception($"Error loading all entities of type '{entityType}' (with tracking): {ex.Message}", ex);
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken token = default)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id && !e.isDeleted, token);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                var entityType = typeof(T);
                var requiredStringProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)));
                var propNames = string.Join(", ", requiredStringProps.Select(p => p.Name));
                throw new Exception($"Error loading entity of type '{entityType.Name}' with Id {id}: {ex.Message}. Possible required string properties that may be null in the database: {propNames}. Check your entity and database schema for mismatches.", ex);
            }
            catch (Exception ex)
            {
                var entityType = typeof(T).Name;
                throw new Exception($"Error loading entity of type '{entityType}' with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<T?> GetIsDeletedByIdAsync(int id, CancellationToken token = default)
        {
            return await _dbSet.IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, token);
        }

        public async Task<T?> AddWithReturningEntityAsync(T entity, CancellationToken token = default)
        {
            var entry = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(token);
            return entry.Entity;
        }


        public async Task<bool> UpdateAsync(T entity, CancellationToken token = default)
        {
            // Find existing entity 
            var existing = await _dbSet
               .AsTracking()
               .FirstOrDefaultAsync(e => e.Id == entity.Id, token);

            if (existing == null) return false;

            // Preserve soft-delete state so an update doesn't accidentally toggle it
            var preservedIsDeleted = existing.isDeleted;
            var preservedDeletedOn = existing.DeletedOnDts;

            // Copy values from incoming entity to tracked existing entity
            _context.Entry(existing).CurrentValues.SetValues(entity);

            // Restore preserved soft-delete state
            existing.isDeleted = preservedIsDeleted;
            existing.DeletedOnDts = preservedDeletedOn;

            await _context.SaveChangesAsync(token);
            return true;
        }



        // Soft delete by setting IsDeleted = true
        public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, token);
            if (entity == null) return false;

            if (!entity.isDeleted)
            {
                entity.isDeleted = true;
                entity.DeletedOnDts = DateTime.UtcNow;
                await _context.SaveChangesAsync(token);
            }

            return true;
        }

        // Restore a soft-deleted entity
        public async Task<bool> RestoreAsync(int id, CancellationToken token = default)
        {
            var entity = await _dbSet.IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, token);

            if (entity == null) return false;
            if (!entity.isDeleted) return false; // or true, depending on your desired semantics

            entity.isDeleted = false;
            entity.DeletedOnDts = null;
            await _context.SaveChangesAsync(token);

            return true;
        }

        // Full delete from the database
        public async Task<bool> FullDeleteAsync(int id, CancellationToken token = default)
        {
            var entity = await _dbSet.IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, token);

            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(token);

            return true;
        }


        public virtual async Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken token = default)
        {
            try
            {
                return await _dbSet.IgnoreQueryFilters().ToListAsync(token);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                var entityType = typeof(T);
                var requiredStringProps = entityType.GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)));
                var propNames = string.Join(", ", requiredStringProps.Select(p => p.Name));
                throw new Exception($"Error loading all (including deleted) entities of type '{entityType.Name}': {ex.Message}. Possible required string properties that may be null in the database: {propNames}. Check your entity and database schema for mismatches.", ex);
            }
            catch (Exception ex)
            {
                var entityType = typeof(T).Name;
                throw new Exception($"Error loading all (including deleted) entities of type '{entityType}': {ex.Message}", ex);
            }
        }



    }
}
