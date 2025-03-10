using System.ComponentModel.DataAnnotations;

namespace Netflex.Models.Serie;

public class CreateSerieModel
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string About { get; set; } = string.Empty;

    [Required(ErrorMessage = "Năm sản xuất là bắt buộc")]
    [Range(1900, 2025, ErrorMessage = "Năm sản xuất không hợp lệ")]
    public int ProductionYear { get; set; }

    [Required(ErrorMessage = "Phải chọn độ tuổi")]
    public Guid AgeCategoryId { get; set; }
    [MaxFileSize(5 * 1024 * 1024), AllowedExtensions([".jpg", ".png", ".jpeg"])]
    public IFormFile Poster { get; set; } = null!;

    public List<Guid> ActorIds { get; set; } = new();
    public List<Guid> GenreIds { get; set; } = new();
    public List<Guid> CountryIds { get; set; } = new();
}