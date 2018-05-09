using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Reactor.Web.Infrastructure.Mvc
{
    public abstract class BaseController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        protected BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)    
        {
            var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var razorViewEngine = _serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = _serviceProvider.GetRequiredService<ITempDataProvider>();
            

            var actionConext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                //search for the view
                var viewResult = razorViewEngine.FindView(actionConext, viewName, false);

                if (!viewResult.Success)
                    throw new Exception(
                        $"view does not exist. These locations were searched {viewResult.SearchedLocations.Join(",")}");

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                var viewContext = new ViewContext(
                    actionConext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(httpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}