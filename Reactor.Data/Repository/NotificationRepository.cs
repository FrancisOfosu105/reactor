using System;
using Reactor.Core.Domain.Notifications;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ReactorDbContext context)
            : base(context)
        {
            if(context==null)
                throw new ArgumentNullException(nameof(context));
        }
    }
}