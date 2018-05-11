using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;


namespace Reactor.Services.ViewRender
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewRenderService(IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider, IRazorViewEngine razorViewEngine)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _razorViewEngine = razorViewEngine;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    )

                    {RouteData = httpContext.GetRouteData()};


                await viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}