using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Chats;
using Reactor.Services.Users;
using Reactor.Web.Models.Chat;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetChatContact([FromForm] string name)
        {
            var user = await _userService.GetUserWithFriendsAsync();


            var friends = !string.IsNullOrWhiteSpace(name)  
                ? user.ApprovedFriends().Where(f =>
                    f.RequestedBy.FullName.ToLower().Contains(name) ||
                    f.RequestedTo.FullName.ToLower().Contains(name))
                : user.ApprovedFriends();

            var chatContacts = new List<ChatContact>();

            foreach (var friend in friends)
            {
                if (friend.RequestedBy.UserName == User.Identity.Name)
                {
                    chatContacts.Add(new ChatContact
                    {
                        FullName = friend.RequestedTo.FullName,
                        ProfilePicture = friend.RequestedTo.GetPicture(),
                        UserId = friend.RequestedTo.Id
                    });
                }
                else
                {
                    chatContacts.Add(new ChatContact
                    {
                        FullName = friend.RequestedBy.FullName,
                        ProfilePicture = friend.RequestedBy.GetPicture(),
                        UserId = friend.RequestedBy.Id
                    });
                }
            }


            return Ok(chatContacts);
        }
      
    }
}