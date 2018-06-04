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
            get => _from ?? DefaultValue;

            set => _from = value;
        }

        public string Lives
        {
            get => _lives ?? DefaultValue;

            set => _lives = value;
        }

        public string WorkAt
        {
            get => _workAt ?? DefaultValue;

            set => _workAt = value;
        }

        public int TotalFollowers { get; set; }

        public int TotalFollowees { get; set; }

        public bool IsProfilePageForUser { get; set; }

        private string DefaultValue => "unknown";
    }
}