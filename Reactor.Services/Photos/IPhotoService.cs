using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Reactor.Services.Photos
{
    public interface IPhotoService
    {
        Task Upload(IFormFileCollection files, int postId);
    }
}