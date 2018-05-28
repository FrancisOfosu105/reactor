using System.Collections.Generic;
using Reactor.Core.Domain.Notifications;

namespace Reactor.Core.Models
{
    public class NotificationTemplateModel
    {
        public bool LoadMore { get; set; }
        
        public IEnumerable<Notification> Notifications { get; set; }
    }
}