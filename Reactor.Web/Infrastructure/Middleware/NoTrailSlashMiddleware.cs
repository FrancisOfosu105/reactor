using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Reactor.Web.Infrastructure.Middleware
{
    public class NoTrailingSlashMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly NoTrailingSlashMiddlewareOptions _options;

        public NoTrailingSlashMiddleware(RequestDelegate next, IOptions<NoTrailingSlashMiddlewareOptions> options)
        {
            _next = next;
            _options = options.Value;
        }


        public async Task Invoke(HttpContext httpContext)
        {
            if (_options.RemoveTrailingSlash)   
            {
                if (httpContext.Request.Path.Value.EndsWith('/'))
                {
                    var index = httpContext.Request.Path.Value.LastIndexOf('/');
                
                    var url = httpContext.Request.Path.Value.Substring(0,index);
                    
                    httpContext.Response.Redirect(url);
                
                }
            }

            await _next.Invoke(httpContext);
        }
    }
}