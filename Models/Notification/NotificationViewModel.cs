namespace Netflex.Models.Notification
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool HaveRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}