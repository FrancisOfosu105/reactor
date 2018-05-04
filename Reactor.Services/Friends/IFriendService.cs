using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Domain.Members;

namespace Reactor.Services.Friends
{
    public interface IFriendService
    {
        Task<IEnumerable<User>> GetSuggestedFriendsAsync();

        Task AddFriendRequestAsync(string requestedById, string requestedToId);

        Task<bool> FriendRequestExistsAsync(string requestedById, string requestedToId);

        Task<IEnumerable<Friend>> GetApprovedFriends();
        
        Task<IEnumerable<Friend>> GetFriendRequests();

        Task AcceptFriendRequestAsync(string requestedByUserId, string requestedToUserId);
    }
}