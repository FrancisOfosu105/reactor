using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Reactor.Core.Domain.Friends;

namespace Reactor.Core.Domain.Members
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }


        public ICollection<Friend> SentFriendRequests { get; set; }

        public ICollection<Friend> ReceievedFriendRequests { get; set; }

        [NotMapped]
        public virtual IEnumerable<Friend> ApprovedFriends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.Approved ).ToList();

                friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));

                return friends;
            }
        }
        [NotMapped]
        public virtual IEnumerable<Friend> NotApprovedFriends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.NotApproved ).ToList();

                friends.AddRange(ReceievedFriendRequests.Where(x => x.NotApproved));

                return friends;
            }
        }
        


        public User()
        {
            SentFriendRequests = new List<Friend>();

            ReceievedFriendRequests = new List<Friend>();
        }
    }
}