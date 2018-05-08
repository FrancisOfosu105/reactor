using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Model.Comments;
using Reactor.Core.Model.Posts;

namespace Reactor.Services.Posts
{
    public interface IPostService
    {
        Task AddPostAsync(Post post);
        
        Task<IEnumerable<PostListModel>> GetPostsAsync();

        Task<Post> GetPostWithCommentsAsync(int postId);

        Task<IEnumerable<Comment>> GetCommentsByIdAsync(int postId);

        Task<DateTime> AddCommentToPost(Post post, string comment);
       
        Task<int> GetTotalCommentsForPostAsnyc(int postId);
        
        Task<Post> GetPostByIdAsync(int postId);
        
        Task<IEnumerable<CommentListModel>> GetPagedComments(int pageIndex, int postId);
    }
}