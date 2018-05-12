using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Follows;
using Reactor.Core.Extensions;
using Reactor.Core.Repository;
using Reactor.Services.Users;

namespace Reactor.Services.Follows
{
    public class FollowService : IFollowService
    {
        private readonly IRepository<Follow> _followRepository;
        private readonly IUserService _userService;

        public FollowService(IRepository<Follow> followRepository, IUserService userService)
        {
            _followRepository = followRepository;
            _userService = userService;
        }

        public async Task FollowUserAsync(string followeeId)
        {
            var followerId = await _userService.GetCurrentUserIdAsync();

            var follow = GetFollow(followerId, followeeId);

            if (follow == null)
            {
                follow = new Follow
                {
                    FolloweeId = followeeId,
                    FollowerId = followerId
                };

                await _followRepository.AddAsync(follow);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(follow)} already exist.");
            }
        }


        public async Task UnFollowUserAsync(string followeeId)
        {
            var followerId = await _userService.GetCurrentUserIdAsync();

            var follow = GetFollow(followerId, followeeId);

            if (follow != null)
            {
                _followRepository.Remove(follow);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(follow)} does not exist.");
            }
        }

        public async Task<IEnumerable<Follow>> GetUserFollowersAsync(string userId)
        {
            return await _followRepository.Table
                .Include(f => f.Follower)
                .Where(f => f.FolloweeId == userId)
                .ToListAsyncSafe();
        }

        public async Task<IEnumerable<Follow>> GetUserFolloweesAsync(string userId)
        {
            return await _followRepository.Table.Include(f => f.Followee).Where(f => f.FollowerId == userId)
                .ToListAsyncSafe();
        }

        public async Task<bool> IsFollowingUserAsync(string followeeId)
        {
            var followerId = await _userService.GetCurrentUserIdAsync();

            return GetFollow(followerId, followeeId) != null;
        }

        public async Task<int> GetUserTotalFollowersAsync(string userId)
        {
            return await _followRepository.Table.Where(f => f.FolloweeId == userId).CountAsync();
        }

        public async Task<int> GetUserTotalFolloweesAsync(string userId)
        {
            return await _followRepository.Table.Where(f => f.FollowerId == userId).CountAsync();
        }

        private Follow GetFollow(string followerId, string followeeId)
        {
            return _followRepository.Table.FirstOrDefault(f =>
                f.FollowerId == followerId && f.FolloweeId == followeeId);
        }
    }
}