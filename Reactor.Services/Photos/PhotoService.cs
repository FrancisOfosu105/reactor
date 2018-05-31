using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task UploadAsync(IFormFileCollection files, int postId)
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

        public async Task<string> UploadAsync(IFormFile file)
        {
            CreateDirectory(Path.Combine(_host.WebRootPath, "uploads/profiles"));

            var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Path.Combine(_host.WebRootPath, "uploads/profiles"), filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/profiles/{filename}";
        }

        public async Task<IEnumerable<Photo>> GetUserPhotosAsync(string userId)
        {
            return await _photoRepository.Table.Where(p => p.Post.CreatedById == userId).ToListAsync();
        }

        public async Task<int> GetUserTotalPhotosAsync(string userId)
        {
            return await _photoRepository.Table.Where(p => p.Post.CreatedById == userId).CountAsync();
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

        public void RemovePhotoFromDisk(string profilePictureUrl)
        {
            if (string.IsNullOrEmpty(profilePictureUrl))
                return;

            var index = profilePictureUrl.IndexOf('/');
            var profilePath = profilePictureUrl.Substring(index + 1);

            var path = Path.Combine(_host.WebRootPath, profilePath);

            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
                File.Delete(path);
        }
    }
}