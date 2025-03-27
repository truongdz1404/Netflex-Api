namespace Netflex.Models.Serie;

public class DetailSerieViewModel
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? About { get; set; }
    public int ProductionYear { get; set; }
    public Guid? AgeCategoryId { get; set; }
    public string? Poster { get; set; }
    public required List<Guid> ActorIds { get; set; }
    public required List<Guid> GenreIds { get; set; }
    public required List<Guid> CountryIds { get; set; }
    public bool IsFollowed { get; set; }
    public DateTime CreatedAt { get; set; }
}