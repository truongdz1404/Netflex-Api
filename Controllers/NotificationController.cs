using System;
using System.Linq;
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
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : BaseController
    {
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

        [HttpGet("unread-count")]
        [Authorize]
        public IActionResult GetUnreadNotificationCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var count = _context.UserNotifications
                .Where(un => un.UserId == userId && !un.HaveRead)
                .Count();

            return Ok(new { unreadCount = count });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var notificationIds = _context.UserNotifications
                .Where(un => un.UserId == userId)
                .Select(un => new { un.NotificationId, un.HaveRead })
                .Distinct()
                .ToList();

            var notifications = _context.Notifications
                .Where(n => notificationIds.Select(x => x.NotificationId).Contains(n.Id))
                .OrderByDescending(x => x.CreatedAt);

            var models = notifications.AsEnumerable().Select(b =>
 {
     var content = JsonSerializer.Deserialize<JsonContent>(b.Content);
     if (content == null) return null;
     return new NotificationViewModel
     {
         Id = b.Id,
         Message = content.Message,
         Link = content.Link,
         HaveRead = notificationIds.FirstOrDefault(x => x.NotificationId == b.Id)?.HaveRead ?? false,
         CreatedAt = b.CreatedAt
     };
 }).Where(m => m != null).ToList();

            // Mark notifications as read
            foreach (var model in models)
            {
                var userNotification = _context.UserNotifications
                    .FirstOrDefault(un => un.UserId == userId && un.NotificationId == model.Id);
                if (userNotification != null)
                {
                    userNotification.HaveRead = true;
                }
            }

            await _context.SaveChangesAsync();

            var pagedList = models.ToPagedList(page, PAGE_SIZE);

            return Ok(new
            {
                data = pagedList,
                pagination = new
                {
                    currentPage = pagedList.PageNumber,
                    totalPages = pagedList.PageCount,
                    pageSize = PAGE_SIZE,
                    totalItems = pagedList.TotalItemCount
                }
            });
        }

        [HttpPost("test")]
        [Authorize]
        public async Task<IActionResult> CreateTestNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(new JsonContent
                {
                    Message = "You have a new notification",
                    Link = "/notification"
                }),
                Status = "System",
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            _context.UserNotifications.Add(new UserNotification
            {
                UserId = userId,
                NotificationId = notification.Id,
                HaveRead = false
            });

            await _context.SaveChangesAsync();
            await _notificationService.PushAsync(new Message(new[] { userId }, "You have a new notification"));

            return Created($"/api/notifications/{notification.Id}", new
            {
                notification.Id,
                notification.Content,
                notification.Status,
                notification.CreatedAt
            });
        }
    }
}