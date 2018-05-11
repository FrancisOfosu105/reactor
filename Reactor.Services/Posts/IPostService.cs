using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;

namespace Reactor.Services.Posts
{
    public interface IPostService
    {
        Task AddPostAsync(Post post);

        Task<Post> GetPostWithCommentsAsync(int postId);

        Task<bool> ShouldPostLoadMoreAsync(string userId = null);

        Task<(IEnumerable<Comment> data, bool loadMore)> GetPagedCommentsByPostIdAsync(int postId, int pageIndex = 1);

        Task AddCommentToPostAsync(Comment comment);    

        Task<Post> GetPostByIdAsync(int postId);

        Task<(IEnumerable<Post> data, bool loadMore)> GetPagedPostsAsync(int pageIndex);

        Task<(IEnumerable<Post> data, bool loadMore)> GetUserPostsAsync(int pageIndex, string userId);

        Task LikePostAsync(int postId);

        Task UnLikePostAsync(int postId);

        Task<int> GetTotalPostLikesExceptCurrentUserAsync(int postId);

        Task<bool> HasUserLikePostAsync(int postId);

        Task<int> GetUserTotalPostsAsync(string userId);
        
        Task<int> GetTotalCommentsForPostAsnyc(int postId);
    }
}