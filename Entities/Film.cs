using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Film : Entity
{
    public required string Title { get; set; }
    public string? About { get; set; }
    public string? Poster { get; set; }
    public string? Path { get; set; }
    public string? Trailer { get; set; }
    public int ProductionYear { get; set; }
    public Guid? AgeCategoryId { get; set; }
    public TimeSpan HowLong { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<FilmActor> FilmActors { get; set; } = [];
    public ICollection<FilmGenre> FilmGenres { get; set; } = [];
    public ICollection<FilmCountry> FilmCountries { get; set; } = [];
}