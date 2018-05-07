using System;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Photos;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        public PhotoRepository(ReactorDbContext context) : base(context)
        {
            if(context ==null) throw new ArgumentNullException(nameof(context));
        }
    }
}