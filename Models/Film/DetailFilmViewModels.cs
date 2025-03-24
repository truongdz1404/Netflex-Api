namespace Netflex.Models.Film;

public class DetailFilmViewModel
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? About { get; set; }
    public string? Poster { get; set; }
    public string? Path { get; set; }
    public string? Trailer { get; set; }
    public int ProductionYear { get; set; }
    public string? AgeCategory { get; set; }
    public bool IsFollowed { get; set; }
}