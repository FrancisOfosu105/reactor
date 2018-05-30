using Microsoft.AspNetCore.Mvc;

namespace Reactor.Web.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}