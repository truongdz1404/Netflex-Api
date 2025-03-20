using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Serie;

public class CreateSerieModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
    public string About { get; set; } = string.Empty;

    [Required(ErrorMessage = "Production year is required")]
    [Range(1900, 2025, ErrorMessage = "Invalid production year")]
    public int ProductionYear { get; set; }

    [Required(ErrorMessage = "Age category must be selected")]
    public Guid AgeCategoryId { get; set; }

    [MaxFileSize(5 * 1024 * 1024), AllowedExtensions([".jpg", ".png", ".jpeg"])]
    public IFormFile Poster { get; set; } = null!;

    public List<Guid> ActorIds { get; set; } = new();
    public List<Guid> GenreIds { get; set; } = new();
    public List<Guid> CountryIds { get; set; } = new();
}
