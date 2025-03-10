namespace Netflex.Models.Blog
{
    public class DetailBlogViewModels
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public string CreaterId { get; set; } = null!;
    }
}
