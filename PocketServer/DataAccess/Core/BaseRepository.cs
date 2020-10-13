using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocketServer.DataAccess.Core
{
    public class BaseRepository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        private readonly DbSet<TEntity> _entitySet;
        private readonly TContext _context;

        public BaseRepository(TContext context)
        {
            _context = context;
            _entitySet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> All()
        {
            return _entitySet.AsQueryable();
        }

        public virtual void Add(TEntity entity)
        {
            _entitySet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            _entitySet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            _entitySet.RemoveRange(entities);
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public virtual Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
