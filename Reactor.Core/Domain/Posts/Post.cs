using System;

namespace Reactor.Core.Domain.Posts
{
    public class Post : EntityBase
    {
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}