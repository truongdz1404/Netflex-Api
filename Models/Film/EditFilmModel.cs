using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Film;

public class EditFilmModel
{
    [Required, StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    [StringLength(1000)]
    public string? About { get; set; }
    [StringLength(1000)]
    public string? Trailer { get; set; }
    [Required]
    public int ProductionYear { get; set; }
    public string? AgeCategoryId { get; set; }
    public IFormFile? Poster { get; set; }
    public IFormFile? File { get; set; }
}