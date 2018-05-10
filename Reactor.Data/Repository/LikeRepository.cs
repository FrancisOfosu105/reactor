using System;
using Reactor.Core.Domain.Likes;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(ReactorDbContext context) : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}