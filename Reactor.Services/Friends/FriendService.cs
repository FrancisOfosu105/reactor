using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Domain.Users;
using Reactor.Core.Extensions;
using Reactor.Core.Repository;
using Reactor.Services.Users;

namespace Reactor.Services.Friends
{
    public class FriendService : IFriendService
    {
        private readonly IRepository<Friend> _friendRepository;
        private readonly IUserService _userService;

        public FriendService(
            IRepository<Friend> friendRepository, IUserService userService)
        {
            _friendRepository = friendRepository;
            _userService = userService;
        }


        public async Task<IEnumerable<User>> GetSuggestedFriendsAsync()
        {
            var users = _userService.GetAllUsersExceptCurrentUser();

            var currentUser = await _userService.GetUserWithFriendsAsync();

            var approvedFriends = currentUser.ApprovedFriends();

            var notApprovedFriends = currentUser.NotApprovedFriends();

            var friends = approvedFriends.Concat(notApprovedFriends).ToList();

            if (!friends.Any())
                return await users.ToListAsyncSafe();

            var suggestedFriends = new List<User>();

            foreach (var user in users)
            {
                if (friends.Any(f => f.RequestedToId == user.Id || f.RequestedById == user.Id))
                    continue;

                suggestedFriends.Add(user);
            }

            return suggestedFriends;
        }

        public async Task AddFriendRequestAsync(string requestedById, string requestedToId)
        {
            var friendRequest = new Friend
            {
                RequestedById = requestedById,
                RequestedToId = requestedToId,
                RequestedOn = DateTime.Now,
                Status = FriendRequestType.None
            };

            var requestedByUser = await _userService.GetUserWithFriendsAsync();

            requestedByUser?.SentFriendRequests.Add(friendRequest);
        }

        public async Task<bool> FriendRequestExistsAsync(string requestedById, string requestedToId)
        {
            var requestedByFriendRequest = await _friendRepository.Table.FirstOrDefaultAsync(f =>
                f.RequestedById == requestedById && f.RequestedToId == requestedToId);

            var requestedToFriendRequest = await _friendRepository.Table.FirstOrDefaultAsync(f =>
                f.RequestedById == requestedToId && f.RequestedToId == requestedById);


            if (requestedToFriendRequest != null)
                return true;

            if (requestedByFriendRequest != null)
                return true;

            return false;
        }

        public async Task<IEnumerable<Friend>> GetApprovedFriends()
        {
            var user = await _userService.GetUserWithFriendsAsync();

            return user.ApprovedFriends();
        }

        public async Task<IEnumerable<Friend>> GetFriendRequests()
        {
            var user = await _userService.GetUserWithFriendsAsync();

            return user.ReceievedFriendRequests.Where(f => f.NotApproved);
        }

        public async Task AcceptFriendRequestAsync(string requestedByUserId, string requestedToUserId)
        {
            var friend = await _friendRepository.Table
                .FirstOrDefaultAsync(f =>
                    f.RequestedById == requestedByUserId && f.RequestedToId == requestedToUserId);

            if (friend == null)
                throw new Exception(nameof(friend));

            friend.Status = FriendRequestType.Approved;
            friend.BecameFriendsOn = DateTime.Now;
        }
    }
}