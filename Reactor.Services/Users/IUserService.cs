using System.Linq;
using System.Threading.Tasks;
using Reactor.Core.Domain.Users;

namespace Reactor.Services.Users
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId);

        Task<User> GetUserNameAsync(string username);

        Task<string> GetCurrentUserIdAsync();
        
        Task<string> GetCurrentUserNameAsync();

        IQueryable<User> GetAllUsersExceptCurrentUser();

        Task<User> GetUserWithFriendsAsync();

        Task<string> GetUserProfilePictureAsync();

        Task<bool> IsProfilePageForCurrentUserAsync(string username);
    }
}