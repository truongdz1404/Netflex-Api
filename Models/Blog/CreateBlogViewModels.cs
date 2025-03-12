using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Netflex.Models.Blog
{
    public class CreateBlogViewModels
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; } = null!;

        [StringLength(20000, ErrorMessage = "Content cannot exceed 20000 characters")]
        public string Content { get; set; } = null!;

        [MaxFileSize(5 * 1024 * 1024), AllowedExtensions([".jpg", ".png", ".jpeg"])]
        public IFormFile? Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Creator ID is required")]
        public string CreaterId { get; set; } = null!;

        // public string? ThumbnailUrl { get; set; }
    }
}
