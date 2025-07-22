using Netflex.Entities.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflex.Entities
{
    public class Comment : Entity
    {
        public string? Content { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Film))]
        public Guid FilmId { get; set; }
        public Film Film { get; set; }

        [ForeignKey(nameof(Serie))]
        public Guid SeriesId { get; set; }
        public Serie Serie { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
