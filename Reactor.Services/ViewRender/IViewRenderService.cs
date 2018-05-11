using System.Threading.Tasks;

namespace Reactor.Services.ViewRender
{
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync(string viewName, object model);
    }
}