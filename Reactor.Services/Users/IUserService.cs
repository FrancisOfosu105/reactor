using System.Linq;
using System.Threading.Tasks;
using Reactor.Core.Domain.Users;

namespace Reactor.Services.Users
{
    public interface IUserService
    {
        Task<User> GetUserAsync(string userId);

        Task<string> GetCurrentUserIdAsync();

        IQueryable<User> GetAllUsers();
        
        IQueryable<User> GetAllUsersExceptCurrentUser();

        Task<User> GetUserWithFriendsAsync();

        Task<string> GetUserProfileAsync();
    }
}