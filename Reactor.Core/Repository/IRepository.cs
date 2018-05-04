using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reactor.Core.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table { get; }
        
        Task AddAsync(T entity);

        void Remove(T entity);
        
        Task<T> FindAsync(Expression<Func<T, object>> predicate);

        Task<IEnumerable<T>> FindAll();
        
    }
    
    
}

