using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Reactor.Core.Domain.Likes;
using Reactor.Core.Repository;
using Reactor.Services.Posts;
using Reactor.Services.Users;

namespace Reactor.Tests.Reactor.Services.Posts
{
    [TestFixture]
    public class PostServiceTests
    {
        private Mock<IPostRepository> _postRepo;
        private Mock<ILikeRepository> _likeRepo;
        private Mock<ICommentRepository> _commentRepo;
        private Mock<IUserService> _userService;
        private PostService _postService;
        private int _postId;
        private string _likeById;
        private Like _like;

        [SetUp]
        public void SetUp()
        {
            _postRepo = new Mock<IPostRepository>();
            _likeRepo = new Mock<ILikeRepository>();
            _commentRepo = new Mock<ICommentRepository>();
            _userService = new Mock<IUserService>();

            _postService = new PostService(_postRepo.Object, _commentRepo.Object, _likeRepo.Object,
                _userService.Object);


            _postId = 25;

            _likeById = "1";
            
            _like = new Like
            {
                LikeById = _likeById,
                PostId = _postId,
                CreatedOn = DateTime.Now
            };
        }

        [Test]
        public async Task LikePost_LikeIsNull_AddLikeToPost()
        {
            _userService.Setup(us => us.GetCurrentUserIdAsync()).ReturnsAsync(_likeById);

            _likeRepo.Setup(m => m.Table).Returns(new List<Like> { }.AsQueryable);

            await _postService.LikePostAsync(_postId);

            _likeRepo.Verify(repo => repo.AddAsync(It.IsAny<Like>()), Times.Once);
        }

        [Test]
        public async Task LikePost_LikeIsNotNull_DontAddLikeToPost()
        {
            _userService.Setup(us => us.GetCurrentUserIdAsync()).ReturnsAsync(_likeById);

           
            _likeRepo.Setup(m => m.Table).Returns(new List<Like>
            {
                _like
            }.AsQueryable);

            await _postService.LikePostAsync(_postId);

            _likeRepo.Verify(repo => repo.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task UnLikePost_LikeIsNotNull_RemoveLikeFromPost()
        {
            _userService.Setup(us => us.GetCurrentUserIdAsync()).ReturnsAsync(_likeById);

            _likeRepo.Setup(m => m.Table).Returns(new List<Like>
            {
                _like
            }.AsQueryable);

            await _postService.UnLikePostAsync(_postId);

            _likeRepo.Verify(repo => repo.Remove(It.IsAny<Like>()), Times.Once);
        }      
        
        [Test]    
        public async Task UnLikePost_LikeIsNull_DontRemoveLikeFromPostBecauseItDoesnotExist()
        {
            _userService.Setup(us => us.GetCurrentUserIdAsync()).ReturnsAsync(_likeById);

            _likeRepo.Setup(m => m.Table).Returns(new List<Like>
            {
                
            }.AsQueryable);

            await _postService.UnLikePostAsync(_postId);

            _likeRepo.Verify(repo => repo.Remove(It.IsAny<Like>()), Times.Never);
        }
    }
}