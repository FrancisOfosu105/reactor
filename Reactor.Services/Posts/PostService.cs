using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Repository;

namespace Reactor.Services.Posts
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;

        public PostService(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task AddPostAsync(Post post)
        {
            await _postRepository.AddAsync(post);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            var posts = _postRepository.Table
                .Include(p => p.CreatedBy)
                .Include(p => p.Photos)
                .OrderByDescending(p => p.CreatedOn);
            return await posts.ToListAsync();
        }
    }
}