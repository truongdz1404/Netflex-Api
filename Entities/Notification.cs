using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Notification : Entity
{
    public required string Content { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}