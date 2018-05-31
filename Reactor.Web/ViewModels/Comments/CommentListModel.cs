using System.Collections.Generic;
using Reactor.Core.Domain.Comments;

namespace Reactor.Web.ViewModels.Comments
{
    public class CommentViewModel
    {
        public IEnumerable<Comment> Comments{ get; set; }

        public int PostId { get; set; }

        public bool LoadMore { get; set; }
    }
}