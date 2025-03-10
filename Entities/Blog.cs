using System.ComponentModel.DataAnnotations;
using Netflex.Entities.Abstractions;

namespace Netflex.Entities;

public class Blog : Entity
{
    public required string Title { get; set; }
    [MaxLength(20000)]
    public required string Content { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string CreaterId { get; set; }
}