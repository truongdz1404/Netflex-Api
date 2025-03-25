using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Actor : Entity
{
    public required string Name { get; set; }
    public string? Photo { get; set; }
    public string? About { get; set; }
}