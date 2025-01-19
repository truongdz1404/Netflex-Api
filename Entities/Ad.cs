using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Ad : Entity
{
    public required string Title { get; set; }
    public required string ContentUrl { get; set; }
    public required string ContentType { get; set; }
    public string? Target { get; set; }
    public int Priority { get; set; }
    public required string Status { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}