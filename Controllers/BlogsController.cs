using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Netflex.Controllers
{
    public class BlogsController : Controller
    {
        // GET: HomeController1
        public ActionResult Index()
        {
            return View();
        }

        // GET: HomeController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        
    }
}
