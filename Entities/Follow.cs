namespace Netflex.Entities;

public class Follow
{
    public required string FollowerId { get; set; }
    public Guid? FilmId { get; set; }
    public Guid? SerieId { get; set; }
    public DateTime FollowedAt { get; set; }
}