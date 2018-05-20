using System;
using System.Collections.Generic;
using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Notifications
{
    public class Notification : EntityBase
    {
        public string SenderId { get; private set; }

        public User Recipient { get; private set; }

        public string RecipientId { get; private set; }

        public DateTime CreatedOn { get; private set; } = DateTime.Now;

        public bool IsRead { get; set; }

        public NotificationType Type { get; private set; }

        public ICollection<NotificationAttribute> Attributes { get; private set; }  

        public Notification()
        {
            Attributes = new List<NotificationAttribute>();
        }

        public Notification(User recipient, string senderId, NotificationType type, List<NotificationAttribute> attributes)
            : this()
        {
            Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
            Type = type;
            Attributes = attributes;
        }
    }
}