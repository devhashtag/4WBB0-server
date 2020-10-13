using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PocketServer.DataAccess.EntityTypes;

namespace PocketServer.DataAccess.Core
{
    public interface IBaseService<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        bool Exists(TKey id);
        Task<bool> ExistsAsync(TKey id);
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);
        IQueryable<TEntity> GetAll();
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void DeleteById(TKey id);
        Task DeleteByIdAsync(TKey id);
        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
