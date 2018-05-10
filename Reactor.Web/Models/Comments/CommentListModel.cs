using System.Collections.Generic;
using Reactor.Core.Domain.Comments;

namespace Reactor.Web.Models.Comments
{
    public class CommentViewModel
    {
        public IEnumerable<Comment> Comments{ get; set; }

        public int PostId { get; set; }

        public bool LoadMore { get; set; }
    }
}