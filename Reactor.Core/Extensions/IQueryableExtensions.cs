using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Reactor.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return !(source is IAsyncEnumerable<TSource>) ? Task.FromResult(source.ToList()) : source.ToListAsync();
        }
    }
}