using System;
using Reactor.Core.Domain.Messages;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(ReactorDbContext context)
            : base(context)
        {
            if (context == null)
                throw new ArgumentException(nameof(context));
        }
    }
}