using Microsoft.AspNetCore.Authorization;

namespace MVC_Demo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

    }
}