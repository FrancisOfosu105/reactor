using System;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Comments
{
    public class Comment :EntityBase
    {
        public User CommentBy { get; set; }
        
        public string CommentById { get; set; }

        public Post Post { get; set; }

        public int PostId { get; set; }

        public string Content { get; set; }    
        
        public DateTime CreatedOn { get; set; }
        
    }
}