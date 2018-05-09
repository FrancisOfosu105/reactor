using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Repository;
using Reactor.Services.Users;

namespace Reactor.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IUserService _userService;
        private readonly IRepository<Comment> _commentRepository;
        private const int PAGE_SIZE = 5;

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

        public Task<Post> GetPostWithCommentsAsync(int postId)
        {
            return _postRepository.Table.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
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

        public async Task<(IEnumerable<Post> data, bool loadMore)> GetPagedPostsAsync(int pageIndex)
        {
            var query = _postRepository.Table
                .Include(p => p.Comments)
                .Include(p => p.Photos)
                .Include(p => p.CreatedBy)
                .OrderByDescending(p => p.CreatedOn)
                .AsQueryable();

            var loadMore = pageIndex * PAGE_SIZE < await query.CountAsync();

            query = query.Skip((pageIndex - 1) * PAGE_SIZE).Take(PAGE_SIZE);

            return (await query.ToListAsync(), loadMore);
        }

        public async Task<bool> ShouldPostLoadMoreAsync()
        {
            var query = _postRepository.Table
                .OrderByDescending(c => c.CreatedOn)
                .AsQueryable();

            return 1 * PAGE_SIZE < await query.CountAsync();
        }

        public async Task<(IEnumerable<Comment> data, bool loadMore)> GetPagedCommentsByPostIdAsync(int postId,
            int pageIndex = 1)
        {
            var query = _commentRepository.Table
                .Include(c => c.Post)
                .Include(c => c.CommentBy)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedOn)
                .AsQueryable();


            var loadMore = pageIndex * PAGE_SIZE < await query.CountAsync();

            query = query.Skip((pageIndex - 1) * PAGE_SIZE).Take(PAGE_SIZE);

            query = query.OrderBy(c => c.CreatedOn).AsQueryable();


            return (await query.ToListAsync(), loadMore);
        }
    }
}