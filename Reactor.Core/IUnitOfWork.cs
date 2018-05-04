using System.Threading.Tasks;

namespace Reactor.Core
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}