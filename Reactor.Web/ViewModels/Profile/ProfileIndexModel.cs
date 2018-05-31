namespace Reactor.Web.ViewModels.Profile
{
    public class ProfileViewModel
    {
        private string _from;
        private string _lives;
        private string _workAt;
        public bool PostLoadMore { get; set; }

        public string From
        {
            get => _from ?? "To Do";

            set => _from = value;
        }

        public string Lives
        {
            get => _lives ?? "To Do";

            set => _lives = value;
        }

        public string WorkAt
        {
            get => _workAt ?? "To Do";

            set => _workAt = value;
        }

        public int TotalFollowers { get; set; }

        public int TotalFollowees { get; set; }

        public bool IsProfilePageForUser { get; set; }
    }
}