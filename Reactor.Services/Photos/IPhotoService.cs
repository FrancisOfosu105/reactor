using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reactor.Core.Domain.Photos;

namespace Reactor.Services.Photos
{
    public interface IPhotoService
    {
        Task Upload(IFormFileCollection files, int postId);
        
        Task<string> Upload(IFormFile file);

        Task<IEnumerable<Photo>> GetUserPhotosAsync(string userId);
      
        Task<int> GetUserTotalPhotosAsync(string userId);
    }
}