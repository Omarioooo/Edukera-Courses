using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Demo.Models;

namespace MVC_Demo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}