using System;
using Reactor.Core.Domain.Chats;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class ChatRepository :Repository<Chat>,IChatRepository
    {
        public ChatRepository(ReactorDbContext context)
            : base(context)
        {
            if(context ==null)
                throw new ArgumentNullException(nameof(context));
        }
    }
}