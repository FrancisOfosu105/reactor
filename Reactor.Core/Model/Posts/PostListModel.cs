using Reactor.Core.Domain.Posts;

namespace Reactor.Core.Model.Posts
{
    public class PostListModel
    {
        public Post Post { get; set; }

        public int  CommentCount { get; set; }
    }
}