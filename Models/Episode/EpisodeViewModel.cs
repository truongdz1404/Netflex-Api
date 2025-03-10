using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Episode;

public class EpisodeViewModel
{
    public Guid Id { get; set; }
    [Required]
    public int Number { get; set; }
    [Required, StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    [StringLength(1000)]
    public string? About { get; set; }
    public string? Path { get; set; }
    public required string Serie { get; set; }
}