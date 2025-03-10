using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Genre : Entity
{
    public required string Name { get; set; }
    public ICollection<SerieGenre> SerieGenres { get; set; } = new List<SerieGenre>();
    public ICollection<SerieGenre> FilmGenres { get; set; } = new List<SerieGenre>();

}