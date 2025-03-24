using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Review
{
    public class ReviewEditModel
    {
        [Range(1, 10)]
        public required int Rating { get; set; }
        public Guid FilmId { get; set; }
    }
}
