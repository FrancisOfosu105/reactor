using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Reactor.Core.Domain.Photos;
using Reactor.Core.Repository;

namespace Reactor.Services.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly IHostingEnvironment _host;
        private readonly IRepository<Photo> _photoRepository;

        public PhotoService(IHostingEnvironment host, IRepository<Photo> photoRepository)
        {
            _host = host;
            _photoRepository = photoRepository;
        }

        public async Task Upload(IFormFileCollection files, int postId)
        {
            CreateDirectory(Path.Combine(_host.WebRootPath, "uploads/posts"));

            foreach (var file in files)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Path.Combine(_host.WebRootPath, "uploads/posts"), filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                await AddPhotoAsync(postId, filename);
            }
        }

        private async Task AddPhotoAsync(int postId, string filename)
        {
            var photo = new Photo
            {
                FileName = filename,
                CreatedOn = DateTime.Now,
                PostId = postId
            };

            await _photoRepository.AddAsync(photo);
        }

        private static void CreateDirectory(string storageLocation)
        {
            if (!Directory.Exists(storageLocation))
            {
                Directory.CreateDirectory(storageLocation);
            }
        }
    }
}