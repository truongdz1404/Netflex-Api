namespace Netflex.Models.Search;

public class SearchItemViewModel
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Title { get; set; }
    public string? About { get; set; }
    public string? Poster { get; set; }
    public int ProductionYear { get; set; }
    public DateTime CreatedAt { get; set; }
}