using System;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Likes
{
    public class Like : EntityBase
    {
        public Post Post { get; set; }

        public int PostId { get; set; }

        public User LikeBy { get; set; }

        public string LikeById { get; set; }

        public DateTime CreatedOn { get; set; }
        
    }
}