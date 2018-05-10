using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reactor.Core.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table { get; }
        
        Task AddAsync(T entity);

        void Remove(T entity);
        
        Task<IEnumerable<T>> FindAll();
        
    }
    
    
}

