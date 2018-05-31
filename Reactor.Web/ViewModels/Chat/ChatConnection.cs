using System.Collections.Generic;

namespace Reactor.Web.ViewModels.Chat
{
    public class ChatConnection
    {
        private readonly List<UserDetail> _connectedUsers = new List<UserDetail>();

        public IEnumerable<UserDetail> UserDetails => _connectedUsers;

        public void AddUserDetail(UserDetail userDetail)
        {
            _connectedUsers.Add(userDetail);
        }

        public void RemoveUserDetail(UserDetail userDetail)
        {
            _connectedUsers.Remove(userDetail);
        }
    }
    
    
    public class UserDetail
    {
        public string UserId { get; set; }
    }
}