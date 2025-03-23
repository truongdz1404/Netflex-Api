namespace Netflex.Models.Notification
{
    public class NotificationViewModel
    {
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}