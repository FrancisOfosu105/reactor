using Microsoft.AspNetCore.Mvc;

namespace Reactor.Web.Controllers
{
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        // GET
        [HttpGet("{username}")]
        public IActionResult Index(string username)
        {
            return View();    
        }
    }
}