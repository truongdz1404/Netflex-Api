using Netflex.Entities.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflex.Entities
{
    public class FavoriteFilms : Entity
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Film))]
        public Guid? FilmId { get; set; }
        public Film Film { get; set; }

        [ForeignKey(nameof(Serie))]
        public Guid? SeriesId { get; set; }
        public Serie Serie { get; set; }
    }
}
