using System;
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

        Task<bool> ShouldPostLoadMoreAsync();

        Task<(IEnumerable<Comment> data, bool loadMore)> GetPagedCommentsByPostIdAsync(int postId, int pageIndex=1);

        Task<DateTime> AddCommentToPost(Post post, string comment);
       
        Task<int> GetTotalCommentsForPostAsnyc(int postId);
        
        Task<Post> GetPostByIdAsync(int postId);

        Task<(IEnumerable<Post> data, bool loadMore)> GetPagedPostsAsync(int pageIndex);

        Task LikePostAsync(int postId);
        
        Task UnLikePostAsync(int postId);

        Task<int> GetTotalPostLikesExceptCurrentUserAsync(int postId);

        Task<bool> HasUserLikePostAsync(int postId);    
    }
}