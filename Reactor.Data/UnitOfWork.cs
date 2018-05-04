using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core;
using Reactor.Data.EfContext;

namespace Reactor.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ReactorDbContext _context;

        public UnitOfWork(ReactorDbContext context)
        {
            _context = context;
        }
        
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}