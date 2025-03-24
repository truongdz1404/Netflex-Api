namespace Netflex.Models.Follow
{
    public class FollowViewModel
    {
        public required string? FollowerId { get; set; }
        public Guid? FilmId { get; set; }
        public Guid? SerieId { get; set; }
        public DateTime FollowedAt { get; set; }

        public virtual Film.DetailFilmViewModel? FollowedFilms { get; set; }
        public virtual Serie.SerieViewModel? FollowedSeries { get; set; }
    }
}


