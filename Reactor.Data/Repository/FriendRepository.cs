using System;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class FriendRepository : Repository<Friend>, IFriendRepository
    {
        public FriendRepository(ReactorDbContext context) 
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}