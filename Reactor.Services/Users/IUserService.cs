using System.Linq;
using System.Threading.Tasks;
using Reactor.Core.Domain.Users;

namespace Reactor.Services.Users
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId);

        Task<User> GetUserByUserNameAsync(string username);

        Task<string> GetCurrentUserIdAsync();

        Task<string> GetCurrentUserNameAsync();

        Task<IQueryable<User>> GetAllUsersExceptCurrentUser();

        Task<User> GetUserWithFriendsAsync(string userId = null);

        Task<string> GetUserProfilePictureAsync();

        Task<bool> IsProfilePageForCurrentUserAsync(string username);

        Task InsertUserSettingAsync(UserSetting userSetting);

        Task<UserSetting> GetUserSettingAsync(string id = null);
    }
}