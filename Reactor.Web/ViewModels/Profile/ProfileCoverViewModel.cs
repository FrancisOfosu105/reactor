namespace Reactor.Web.ViewModels.Profile
{
    public class ProfileCoverViewModel
    {
        public string FullName { get; set; }
        
        public string ProfilePicture { get; set; }
        
        public string ProfileCoverPicture { get; set; }    

        public int TotalPosts { get; set; }

        public int TotalPhotos { get; set; }

        public int TotalFollowers { get; set; }

        public int TotalFollowees { get; set; }

        public bool IsFollowingUser { get; set; }

        public string Description { get; set; }
    }
}