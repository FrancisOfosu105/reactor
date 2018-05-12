namespace Reactor.Web.Models.Profile
{
    public class ProfileCoverModel
    {
        public string FullName { get; set; }
        
        public string ProfilePicture { get; set; }

        public int TotalPosts { get; set; }

        public int TotalPhotos { get; set; }

        public int TotalFollowers { get; set; }

        public int TotalFollowees { get; set; }

        public bool IsFollowingUser { get; set; }
    }
}