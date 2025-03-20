using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflex.Models.Blog;

public partial class BlogViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreaterId { get; set; } = null!;

    public string? CreatorName { get; set; }
}
