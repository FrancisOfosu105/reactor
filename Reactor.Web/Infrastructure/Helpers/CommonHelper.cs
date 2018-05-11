using Microsoft.AspNetCore.Http;

namespace Reactor.Web.Infrastructure.Helpers
{
    public class CommonHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CommonHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUserNameFromUrl()
        {
            var url = _contextAccessor.HttpContext.Request.Path.Value;
            
            var index = url.LastIndexOf('/');
            
            return url.Substring(index + 1);
        }
    }
}