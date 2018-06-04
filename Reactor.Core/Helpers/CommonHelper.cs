using System;
using Microsoft.AspNetCore.Http;

namespace Reactor.Core.Helpers
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

        public static int GenerateRandomValue(int limit)
        {
            limit += 1;
            
            var rnd = new Random();

            return rnd.Next(1, limit);
        }
    }
}