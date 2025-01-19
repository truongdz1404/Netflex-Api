namespace Netflex.Entities;

public class FilmFollow
{
    public required string UserId { get; set; }
    public required Guid FilmId { get; set; }
}