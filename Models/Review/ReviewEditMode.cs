using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Review
{
    public class ReviewEditModel
    {
        [Range(1, 5)]
        public required int Rating { get; set; }
        public Guid? FilmId { get; set; }
        public Guid SerieId { get; set; }

        public required Guid CreaterId { get; set; } 

    }
}
