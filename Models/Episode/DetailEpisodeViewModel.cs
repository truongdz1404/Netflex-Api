using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Episode;

public class DetailEpisodeViewModel
{
    [Required]
    public int Number { get; set; }
    [Required, StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    [StringLength(1000)]
    public string? About { get; set; }
    public string? File { get; set; }
    public required string Serie { get; set; }
}