using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Country : Entity
{
    public required string Name { get; set; }
    public ICollection<SerieCountry> SerieCountries { get; set; } = new List<SerieCountry>();
    public ICollection<FilmCountry> FilmCountries { get; set; } = new List<FilmCountry>();
}