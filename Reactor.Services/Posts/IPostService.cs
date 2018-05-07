using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Posts;

namespace Reactor.Services.Posts
{
    public interface IPostService
    {
        Task AddPostAsync(Post post);
        
        Task<IEnumerable<Post>> GetPostsAsync();    
    }
}