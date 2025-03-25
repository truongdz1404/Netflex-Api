using Netflex.Entities.Abstractions;

namespace Netflex.Entities
{
    public class Serie : Entity
    {
        public required string Title { get; set; }
        public string About { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public int ProductionYear { get; set; }
        public Guid? AgeCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<SerieActor> SerieActors { get; set; } = [];
        public ICollection<SerieGenre> SerieGenres { get; set; } = [];
        public ICollection<SerieCountry> SerieCountries { get; set; } = [];
    }
}
