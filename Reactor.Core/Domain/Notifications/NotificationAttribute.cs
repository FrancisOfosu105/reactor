namespace Reactor.Core.Domain.Notifications
{
    public class NotificationAttribute : EntityBase
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public Notification Notification { get; set; }

        public int NotificationId { get; set; } 

    }
}