using Microsoft.AspNetCore.Mvc;

namespace Netflex.Controllers
{
    public class GenreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
