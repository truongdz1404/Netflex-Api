using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Netflex.Database;
using Netflex.Models.Notification;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class NotificationController : BaseController
    {
        private const string Read = "read";
        private const string Unread = "unread";
        private readonly ApplicationDbContext _context;

        private const int PAGE_SIZE = 6;

        public NotificationController(IUnitOfWork unitOfWork, ApplicationDbContext context) : base(unitOfWork)
        {
            _context = context;
        }

        private class JsonContent
        {
            public string Message { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
        }

        public IActionResult Index(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notificationId = _context.UserNotifications
                .Where(un => un.UserId == userId)
                .Select(un => un.NotificationId).Distinct().ToList();

            var notifications = _context.Notifications.Where(n => notificationId.Contains(n.Id))
                .OrderBy(x => x.CreatedAt).AsQueryable();

            int pageNumber = page ?? 1;

            var models = new List<NotificationViewModel>();
            foreach (var b in notifications.OrderBy(b => b.CreatedAt))
            {
                var content = JsonSerializer.Deserialize<JsonContent>(b.Content);
                if (content == null) continue;
                models.Add(new NotificationViewModel
                {
                    Message = content.Message,
                    Link = content.Link,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                });
            }
            return View(models.ToPagedList(pageNumber, PAGE_SIZE));
        }
    }
}