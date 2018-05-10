using System;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class CommentRepository :Repository<Comment>,ICommentRepository
    {
        public CommentRepository(ReactorDbContext context) : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

        }
    }
}