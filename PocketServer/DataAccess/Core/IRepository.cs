using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Core
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> All();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        void SaveChanges();

        Task SaveChangesAsync();
    }
}
