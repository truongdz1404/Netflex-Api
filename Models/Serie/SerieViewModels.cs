namespace Netflex.Models.Serie;

public class SerieViewModel
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    public string? About { get; set; }
    public int ProductionYear { get; set; }
    public Guid? AgeCategoryId { get; set; }
    public string? Poster { get; set; }
}