using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Reactor.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return !(source is IAsyncEnumerable<TSource>) ? Task.FromResult(source.ToList()) : source.ToListAsync();
        }

        public static IQueryable<T> ApplyingPagination<T>(this IQueryable<T> query, int pageIndex = 1,
            int pageSize = 10)
        {
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static bool ShouldEnableLoadMore<T>(this IQueryable<T> query, int pageIndex = 1, int pageSize = 10)
        {
            return pageIndex * pageSize <  query.Count();
        }
    }
}