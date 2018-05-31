namespace Reactor.Web.ViewModels.SettingsViewModel
{
    public class NotificationViewModel
    {
        public bool NotifyWhenUserCommentOnPost { get; set; }

        public bool NotifyWhenUserLikePost { get; set; }

        public bool NotifyWhenUserFollow { get; set; }

        public bool NotifyWhenUserUnFollow { get; set; }

        public bool NotifyWhenUserSendFriendRequest { get; set; }

        public bool NotifyWhenUserAcceptFriendRequest { get; set; }

        public bool NotifyWhenUserRejectFriendRequest { get; set; }
    }
}