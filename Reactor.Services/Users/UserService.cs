using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Users;

namespace Reactor.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<User> _userManager;

        public UserService(IHttpContextAccessor accessor, UserManager<User> userManager)
        {
            _accessor = accessor;
            _userManager = userManager;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            return await _userManager.GetUserIdAsync(user);
        }

        public async Task<string> GetCurrentUserNameAsync()
        {
            var user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            return await _userManager.GetUserNameAsync(user);
        }

        public async Task<IQueryable<User>> GetAllUsersExceptCurrentUser()
        {
            var currentUserId = await GetCurrentUserIdAsync();
            
            return _userManager.Users.Where(u => u.Id != currentUserId);
        }

        public async Task<User> GetUserWithFriendsAsync(string userId =null)
        {
            if (userId != null)
                return await _userManager.Users
                    .Include(u => u.SentFriendRequests)
                    .ThenInclude(f => f.RequestedTo)
                    .Include(u => u.ReceievedFriendRequests)
                    .ThenInclude(f => f.RequestedBy)
                    .FirstOrDefaultAsync(u => u.Id == userId);
             
   var currentUserId = await GetCurrentUserIdAsync();
            
                return await _userManager.Users
                    .Include(u => u.SentFriendRequests)
                    .ThenInclude(f => f.RequestedTo)
                    .Include(u => u.ReceievedFriendRequests)
                    .ThenInclude(f => f.RequestedBy)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);

        }

        public async Task<string> GetUserProfilePictureAsync()
        {
            var user = await GetUserByIdAsync(await GetCurrentUserIdAsync());

            return user.GetPicture();
        }

        public async Task<bool> IsProfilePageForCurrentUserAsync(string username)
        {
            var currentUser = await GetUserByIdAsync(await GetCurrentUserIdAsync());

            return string.Equals(currentUser.UserName, username,StringComparison.OrdinalIgnoreCase);
        }
    }
}