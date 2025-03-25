using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Country : Entity
{
    public required string Name { get; set; }
}