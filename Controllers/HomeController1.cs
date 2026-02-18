using Microsoft.AspNetCore.Mvc;

namespace AgroMonitor.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
