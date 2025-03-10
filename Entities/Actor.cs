using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Actor : Entity
{
    public required string Name { get; set; }
    public string? Photo { get; set; }
    public string? About { get; set; }
    public ICollection<SerieActor> SerieActors { get; set; } = new List<SerieActor>();
    public ICollection<FilmActor> FilmActors { get; set; } = new List<FilmActor>();
}