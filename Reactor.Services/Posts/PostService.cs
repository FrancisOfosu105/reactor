using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Model.Comments;
using Reactor.Core.Model.Posts;
using Reactor.Core.Repository;
using Reactor.Services.Users;

namespace Reactor.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IUserService _userService;
        private readonly IRepository<Comment> _commentRepository;

        public PostService(IRepository<Post> postRepository, IUserService userService,
            IRepository<Comment> commentRepository)
        {
            _postRepository = postRepository;
            _userService = userService;
            _commentRepository = commentRepository;
        }

        public async Task AddPostAsync(Post post)
        {
            await _postRepository.AddAsync(post);
        }

        public async Task<IEnumerable<PostListModel>> GetPostsAsync()
        {
            var posts = _postRepository.Table
                .Include(p => p.CreatedBy)
                .Include(p => p.Photos)
                .OrderByDescending(p => p.CreatedOn).Select(p=> new PostListModel
                {
                    Post = p,
                    CommentCount = p.Comments.Count
                } );

            return await posts.ToListAsync();
        }

        public Task<Post> GetPostWithCommentsAsync(int postId)
        {
            return _postRepository.Table.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
        }


        public async Task<IEnumerable<Comment>> GetCommentsByIdAsync(int postId)
        {
            var comments = _commentRepository.Table
                .Include(c => c.Post)
                .Include(c => c.CommentBy)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedOn)
                .Take(5);

            comments = comments.OrderBy(c => c.CreatedOn);

            return await comments.ToListAsync();
        }

        public async Task<DateTime> AddCommentToPost(Post post, string comment)
        {
            var com = new Comment
            {
                CommentById = await _userService.GetCurrentUserIdAsync(),
                CreatedOn = DateTime.Now,
                Content = comment
            };
            post.Comments.Add(com);

            return com.CreatedOn;
        }

        public async Task<int> GetTotalCommentsForPostAsnyc(int postId)
        {
            return await _commentRepository.Table.Where(c => c.PostId == postId).CountAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _postRepository.Table.FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<IEnumerable<CommentListModel>> GetPagedComments(int pageIndex, int postId)
        {
            var comments =  _commentRepository.Table
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedOn)
                .Skip((pageIndex - 1) * 5)
                .Take(5).Select(c=>new CommentListModel
                {
                    Comment = c.Content,
                    FullName = c.CommentBy.FullName,
                    CreatedOn = c.CreatedOn.ToString("o"),
                    ProfilePicture = c.CommentBy.GetPicture()
                });

            comments = comments.OrderBy(c => c.CreatedOn);

            return await comments.ToListAsync();
        }
    }
}