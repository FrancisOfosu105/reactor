using System;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Follows;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class FollowRepository: Repository<Follow> , IFollowRepository
    {
        public FollowRepository(ReactorDbContext context)
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}