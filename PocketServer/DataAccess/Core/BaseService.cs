using PocketServer.DataAccess.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace PocketServer.DataAccess.Core
{
    public class BaseService<TEntity, TKey> : IBaseService<TEntity, TKey> where TEntity: IEntity<TKey>, new()
    {
        private readonly IRepository<TEntity> _repository;

        public BaseService(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public bool Exists(TKey id)
        {
            return _repository.All().Any(o => o.Id.Equals(id));
        }

        public Task<bool> ExistsAsync(TKey id)
        {
            return _repository.All().AnyAsync(o => o.Id.Equals(id));
        }

        public TEntity GetById(TKey id)
        {
            return _repository.All().SingleOrDefault(o => o.Id.Equals(id));
        }

        public Task<TEntity> GetByIdAsync(TKey id)
        {
            return _repository.All().SingleOrDefaultAsync(o => o.Id.Equals(id));
        }

        public IQueryable<TEntity> GetAll()
        {
            return _repository.All();
        }

        public TEntity Add(TEntity entity)
        {
            _repository.Add(entity);
            _repository.SaveChanges();

            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            _repository.Add(entity);
            await _repository.SaveChangesAsync();

            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            _repository.Update(entity);
            _repository.SaveChanges();

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity;
        }

        public void Delete(TEntity entity)
        {
            _repository.Delete(entity);
            _repository.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public void DeleteById(TKey id)
        {
            _repository.Delete(new TEntity() { Id = id });
            _repository.SaveChanges();
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            _repository.Delete(new TEntity() { Id = id });
            await _repository.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _repository.DeleteRange(entities);
        }
    }
}
