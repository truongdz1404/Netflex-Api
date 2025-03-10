using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Film;

public class CreateFilmViewModel
{
    [Required, StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    [StringLength(1000)]
    public string? About { get; set; }
    [RegularExpression(@"^https:\/\/www\.youtube\.com\/watch\?v=(.*?)(?:\&(.*))?$", ErrorMessage = "Trailer must be a valid Youtube link")]
    [StringLength(1000)]
    public string? Trailer { get; set; }
    [Required, Range(1, int.MaxValue, ErrorMessage = "Production year must be greater than 0")]
    public int ProductionYear { get; set; }
    [Required(ErrorMessage = "The Age category field is required.")]
    public string? AgeCategoryId { get; set; }
    [MaxFileSize(5 * 1024 * 1024), AllowedExtensions([".jpg", ".png", ".jpeg"])]
    public IFormFile? Poster { get; set; }
    [MaxFileSize(5 * 1024 * 1024), AllowedExtensions([".m3u8"])]
    public IFormFile? File { get; set; }
    public List<Guid> ActorIds { get; set; } = new();
    public List<Guid> GenreIds { get; set; } = new();
    public List<Guid> CountryIds { get; set; } = new();
}