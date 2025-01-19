namespace Netflex.Entities;

public class UserNotification
{
    public required string UserId { get; set; }
    public required Guid NotificationId { get; set; }
}