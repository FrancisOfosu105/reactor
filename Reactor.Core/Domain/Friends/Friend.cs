using System;
using System.ComponentModel.DataAnnotations.Schema;
using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Friends
{
    public class Friend
    {
        public User RequestedBy { get; set; }

        public User RequestedTo { get; set; }

        public string RequestedById { get; set; }

        public string RequestedToId { get; set; }

        public DateTime? RequestedOn { get; set; }
    
        public DateTime? BecameFriendsOn { get; set; }    

        public FriendRequestType Status { get; set; }

        [NotMapped] public bool NotApproved => Status == FriendRequestType.None;
        
        [NotMapped] public bool Approved => Status == FriendRequestType.Approved;

        [NotMapped] public bool Rejected => Status == FriendRequestType.Rejected;
    }
}