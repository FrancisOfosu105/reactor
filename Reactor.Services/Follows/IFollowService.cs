using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Follows;

namespace Reactor.Services.Follows
{
    public interface IFollowService
    {
        Task FollowUserAsync(string followeeId);

        Task UnFollowUserAsync(string followeeId);
        
        Task<IEnumerable<Follow>> GetUserFollowersAsync(string userId);  
        
        Task<IEnumerable<Follow>> GetUserFolloweesAsync(string userId);
        
        Task<bool> IsFollowingUserAsync(string followeeId);

        Task<int> GetUserTotalFollowersAsync(string userId);
        
        Task<int> GetUserTotalFolloweesAsync(string userId);
    }
}