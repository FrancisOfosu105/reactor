using Microsoft.AspNetCore.Builder;
using Reactor.Web.Infrastructure.Middleware;

namespace Reactor.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNoTrailingSlash(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoTrailingSlashMiddleware>();
        }
    }
}