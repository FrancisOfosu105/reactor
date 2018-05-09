using System.Collections.Generic;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;

namespace Reactor.Web.Models.Comments
{
    public class CommentViewModel
    {
        public IEnumerable<Comment> Comments{ get; set; }

        public int PostId { get; set; }

        public bool LoadMore { get; set; }
    }
}