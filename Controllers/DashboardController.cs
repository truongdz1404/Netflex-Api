using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflex.Database;
using Netflex.Hubs;
using Netflex.Models.Dashboard;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly ConnectionManager _connection;

        public DashboardController(IUnitOfWork unitOfWork, ConnectionManager connection, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _connection = connection;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            var todayFilms = _unitOfWork.Repository<Film>().Entities
                .Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date)
                .Count();

            var todaySeries = _unitOfWork.Repository<Serie>().Entities
                .Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date)
                .Count();

            var todayBlogs = _unitOfWork.Repository<Blog>().Entities
                .Where(x => x.CreatedAt.Date == DateTime.UtcNow.Date)
                .Count();

            var onlineUsers = _connection.OnlineUsers.Count();

            var monthlyFilmsUploads = _unitOfWork.Repository<Film>().Entities
                .Where(x => x.CreatedAt >= DateTime.UtcNow.AddMonths(-11).Date)
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var monthlySeriesUploads = _unitOfWork.Repository<Serie>().Entities
                .Where(x => x.CreatedAt >= DateTime.UtcNow.AddMonths(-11).Date)
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var dashboardStats = new DashboardViewModel
            {
                TodayFilms = todayFilms,
                TodaySeries = todaySeries,
                TodayBlogs = todayBlogs,
                OnlineUsers = onlineUsers
            };

            return Ok(new
            {
                dashboardStats,
                monthlyFilmsUploads,
                monthlySeriesUploads
            });
        }

        [HttpGet("media")]
        public IActionResult GetMediaStatics()
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddMonths(-11);

            var films = _unitOfWork.Repository<Film>().Entities
                .Where(f => f.CreatedAt >= startDate && f.CreatedAt <= now)
                .GroupBy(f => new { f.CreatedAt.Year, f.CreatedAt.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .ToList();

            var series = _unitOfWork.Repository<Serie>().Entities
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= now)
                .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .ToList();

            var labels = new List<string>();
            var filmData = new List<int>();
            var serieData = new List<int>();

            for (int i = 11; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var label = month.ToString("MM/yyyy");
                labels.Add(label);

                var filmCount = films.FirstOrDefault(f => f.Month == month.Month && f.Year == month.Year)?.Count ?? 0;
                var serieCount = series.FirstOrDefault(s => s.Month == month.Month && s.Year == month.Year)?.Count ?? 0;

                filmData.Add(filmCount);
                serieData.Add(serieCount);
            }

            return Ok(new { labels, filmData, serieData });
        }

        [HttpGet("blog-statics")]
        public IActionResult GetBlogStatics()
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddMonths(-11);

            var blogs = _unitOfWork.Repository<Blog>().Entities
                .Where(f => f.CreatedAt >= startDate && f.CreatedAt <= now)
                .GroupBy(f => new { f.CreatedAt.Year, f.CreatedAt.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .ToList();

            var labels = new List<string>();
            var data = new List<int>();

            for (int i = 11; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var label = month.ToString("MM/yyyy");
                labels.Add(label);

                var blogCount = blogs.FirstOrDefault(f => f.Month == month.Month && f.Year == month.Year)?.Count ?? 0;

                data.Add(blogCount);
            }

            return Ok(new { labels, data });
        }
    }
}