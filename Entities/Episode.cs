using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Episode : Entity
{
    public int Number { get; set; }
    public required string Title { get; set; }
    public string? About { get; set; }
    public required string Path { get; set; }
    public TimeSpan HowLong { get; set; }
    public required Guid SerieId { get; set; }
}