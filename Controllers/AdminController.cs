using Microsoft.AspNetCore.Mvc;

namespace Netflex.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
