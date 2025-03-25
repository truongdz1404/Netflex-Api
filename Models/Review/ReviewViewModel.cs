namespace Netflex.Models.Review
{
    public class ReviewViewModel
    {
        public double AverageRating { get; set; }
        public int Rating { get; set; }
        public int TotalReviews { get; set; }
        public Guid? FilmId { get; set; }
        public Guid? SerieId { get; set; }
    }
}
