using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Likes;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Extensions;
using Reactor.Core.Repository;
using Reactor.Services.Users;

namespace Reactor.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IUserService _userService;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Like> _likeRepository;

        public PostService(
            IRepository<Post> postRepository,
            IRepository<Comment> commentRepository,
            IRepository<Like> likeRepository,
            IUserService userService
        )
        {
            _postRepository = postRepository;
            _userService = userService;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
        }

        public async Task AddPostAsync(Post post)
        {
            await _postRepository.AddAsync(post);
        }

        public Task<Post> GetPostWithCommentsAsync(int postId)
        {
            return _postRepository.Table.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
        }

        public Task<Post> GetPostWithUserAsync(int postId)
        {
            return _postRepository.Table.Include(p => p.CreatedBy).ThenInclude(u=>u.Notifications).FirstOrDefaultAsync(p => p.Id == postId);
        }


        public async Task AddCommentToPostAsync(Comment comment)
        {
            await _commentRepository.AddAsync(comment);
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
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedOn)
                .AsQueryable();

            var loadMore = query.ShouldEnableLoadMore(pageIndex: pageIndex);

            query = query.ApplyingPagination(pageIndex);

            return (await query.ToListAsync(), loadMore);
        }

        public async Task<(IEnumerable<Post> data, bool loadMore)> GetUserPostsAsync(int pageIndex, string userId)
        {
            var query = _postRepository.Table
                .Include(p => p.Comments)
                .Include(p => p.Photos)
                .Include(p => p.CreatedBy)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedOn)
                .Where(p => p.CreatedById == userId)
                .AsQueryable();

            var loadMore = query.ShouldEnableLoadMore(pageIndex: pageIndex);

            query = query.ApplyingPagination(pageIndex);

            return (await query.ToListAsync(), loadMore);
        }


        public async Task LikePostAsync(int postId)
        {
            var userId = await _userService.GetCurrentUserIdAsync();

            var like = _likeRepository.Table.FirstOrDefault(l => l.PostId == postId && l.LikeById == userId);

            if (like == null)
            {
                like = new Like
                {
                    LikeById = userId,
                    PostId = postId,
                    CreatedOn = DateTime.Now
                };

                await _likeRepository.AddAsync(like);
            }
        }


        public async Task UnLikePostAsync(int postId)
        {
            var userId = await _userService.GetCurrentUserIdAsync();

            var like = _likeRepository.Table.FirstOrDefault(l => l.PostId == postId && l.LikeById == userId);

            if (like != null)
            {
                _likeRepository.Remove(like);
            }
        }

        public async Task<int> GetTotalPostLikesExceptCurrentUserAsync(int postId)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            return await _likeRepository.Table.Where(l => l.PostId == postId && l.LikeById != userId).CountAsync();
        }

        public async Task<bool> HasUserLikePostAsync(int postId)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            return await _likeRepository.Table.AnyAsync(l => l.PostId == postId && l.LikeById == userId);
        }

        public async Task<int> GetUserTotalPostsAsync(string userId)
        {
            return await _postRepository.Table.Where(p => p.CreatedById == userId).CountAsync();
        }

        public async Task<int> GetTotalCommentsForPostAsnyc(int postId)
        {
            return await _commentRepository.Table.Where(c => c.PostId == postId).CountAsync();
        }

        public Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return _commentRepository.Table.FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public bool ShouldPostLoadMore(string userId = null)
        {
            IQueryable<Post> query;

            if (userId == null)
            {
                query = _postRepository.Table
                    .OrderByDescending(c => c.CreatedOn)
                    .AsQueryable();
            }
            else
            {
                query = _postRepository.Table
                    .Where(p => p.CreatedById == userId)
                    .OrderByDescending(c => c.CreatedOn)
                    .AsQueryable();
            }


            return query.ShouldEnableLoadMore();
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


            var loadMore = query.ShouldEnableLoadMore(pageIndex: pageIndex);

            query = query.ApplyingPagination(pageIndex);

            query = query.OrderBy(c => c.CreatedOn);


            return (await query.ToListAsync(), loadMore);
        }
    }
}