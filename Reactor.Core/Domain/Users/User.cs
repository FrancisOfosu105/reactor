using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Follows;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Domain.Likes;
using Reactor.Core.Domain.Notifications;
using Reactor.Core.Domain.Posts;

namespace Reactor.Core.Domain.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string ProfileCoverPictureUrl { get; set; }

        public string Description { get; set; }

        public string Lives { get; set; }

        public string From { get; set; }

        public string WorkAt { get; set; }

        public ICollection<Friend> SentFriendRequests { get; set; }

        public ICollection<Friend> ReceievedFriendRequests { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Follow> Followers { get; set; }

        public ICollection<Follow> Followees { get; set; }

        [NotMapped] public string FullName => $"{FirstName} {LastName}";

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Like> Likes { get; set; }

        public ICollection<Notification> Notifications { get; private set; }

        public UserSetting UserSetting { get; set; }

        public User()
        {
            SentFriendRequests = new List<Friend>();

            ReceievedFriendRequests = new List<Friend>();

            Posts = new List<Post>();

            Comments = new List<Comment>();

            Likes = new List<Like>();

            Followers = new List<Follow>();

            Followees = new List<Follow>();

            Notifications = new List<Notification>();
        }

        public virtual IEnumerable<Friend> ApprovedFriends()
        {
            var friends = SentFriendRequests.Where(x => x.Approved).ToList();

            friends.AddRange(ReceievedFriendRequests.Where(x => x.Approved));

            return friends;
        }

        public virtual IEnumerable<Friend> NotApprovedFriends()
        {
            var friends = SentFriendRequests.Where(x => x.NotApproved).ToList();

            friends.AddRange(ReceievedFriendRequests.Where(x => x.NotApproved));

            return friends;
        }

        public string GetProfilePicture()
        {
            return ProfilePictureUrl ?? "/assets/images/no-profile.svg";
        }


        public string GetProfileCoverPicture()
        {
            return ProfileCoverPictureUrl ?? "/assets/images/no-profile-cover.jpg";
        }

        public void CreateNotification(Notification notification)
        {
            Notifications.Add(notification);
        }

        public void RemoveNotification(Notification notification)
        {
            Notifications.Remove(notification);
        }
    }
}