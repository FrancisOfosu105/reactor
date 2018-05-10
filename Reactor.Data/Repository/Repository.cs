using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Repository;

namespace Reactor.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        protected Repository(DbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Table => _dbSet;

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> FindAll()
        {
            return await _dbSet.ToListAsync();
        }
    }
}