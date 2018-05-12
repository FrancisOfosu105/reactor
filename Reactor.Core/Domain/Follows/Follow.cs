using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Follows
{
    public class Follow 
    {
        public User Follower { get; set; }

        public User Followee { get; set; }

        public string FollowerId { get; set; }

        public string FolloweeId { get; set; }
    }
}