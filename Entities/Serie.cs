using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Serie : Entity
{
    public required string Title { get; set; }
    public string? About { get; set; }
    public string? Poster { get; set; }
    public int ProductionYear { get; set; }
    public Guid? AgeCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}