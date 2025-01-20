namespace Netflex.Entities;

public class Review
{
    public required int Rating { get; set; }
    public required string CreaterId { get; set; }
    public Guid? FilmId { get; set; }
    public Guid? SerieId { get; set; }
}