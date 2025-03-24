using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly NotificationQueueService _notificationService;

        private const int PAGE_SIZE = 6;

        public NotificationController(IUnitOfWork unitOfWork, ApplicationDbContext context,
            NotificationQueueService notificationService) : base(unitOfWork)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private class JsonContent
        {
            public string Message { get; set; } = string.Empty;
            public string Link { get; set; } = string.Empty;
        }


        [HttpGet("/notification/unread")]
        public IActionResult CountUnreadNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = _context.UserNotifications
                .Where(un => un.UserId == userId && un.HaveRead == false)
                .Count();
            return Json(count);
        }

        public IActionResult IndexAsync(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notificationId = _context.UserNotifications
                .Where(un => un.UserId == userId)
                .Select(un => new { un.NotificationId, un.HaveRead }).Distinct().ToList();

            var notifications = _context.Notifications.Where(n => notificationId.Select(x => x.NotificationId).Contains(n.Id))
                .OrderBy(x => x.CreatedAt).AsQueryable();

            int pageNumber = page ?? 1;

            var models = new List<NotificationViewModel>();
            foreach (var b in notifications.OrderBy(b => b.CreatedAt))
            {
                var content = JsonSerializer.Deserialize<JsonContent>(b.Content);
                if (content == null) continue;
                models.Add(new NotificationViewModel
                {
                    Id = b.Id,
                    Message = content.Message,
                    Link = content.Link,
                    HaveRead = notificationId.FirstOrDefault(x => x.NotificationId == b.Id)?.HaveRead ?? false,
                    CreatedAt = b.CreatedAt
                });
            }
            foreach (var model in models)
            {
                var userNotification = _context.UserNotifications.FirstOrDefault(un => un.UserId == userId && un.NotificationId == model.Id);
                if (userNotification == null) continue;
                userNotification.HaveRead = true;
            }
            _context.SaveChanges();
            return View(models.ToPagedList(pageNumber, PAGE_SIZE));
        }

        [HttpPost("/notification/test")]
        public async Task<IActionResult> TestNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
                await _notificationService.PushAsync(new Message([userId], "You have a new notification"));
            return Ok();
        }
    }
}