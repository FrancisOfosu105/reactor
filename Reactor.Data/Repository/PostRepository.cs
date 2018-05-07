using System;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Posts;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ReactorDbContext context) 
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}