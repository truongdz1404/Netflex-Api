using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflex.Hubs;
using Netflex.Models.Dashboard;

namespace Netflex.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ConnectionManager _connection;
        public DashboardController(IUnitOfWork unitOfWork, ConnectionManager connection)
        {
            _unitOfWork = unitOfWork;
            _connection = connection;
        }
        public IActionResult Index()
        {
            var todayFilms = _unitOfWork.Repository<Film>().Entities.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).Count();
            var todaySeries = _unitOfWork.Repository<Serie>().Entities.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).Count();
            var todayBlogs = _unitOfWork.Repository<Blog>().Entities.Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date).Count();
            var onlineUsers = _connection.OnlineUsers.Count();
            return View(new DashboardViewModel { TodayFilms = todayFilms, TodaySeries = todaySeries, TodayBlogs = todayBlogs, OnlineUsers = onlineUsers });
        }
    }
}
