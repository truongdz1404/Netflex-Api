using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;

namespace Netflex.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 6;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var query = _context.Blogs
                .Select(b => new BlogViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                    CreatorName = _context.Users
                        .Where(u => u.Id == b.CreaterId)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                });

            var totalItems = await query.CountAsync();

            var blogs = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            return Ok(new
            {
                PageNumber = page,
                PageSize = PAGE_SIZE,
                TotalItems = totalItems,
                Blogs = blogs
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var blog = await _context.Blogs
                .Where(b => b.Id == id)
                .Select(b => new DetailBlogViewModels
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                    CreatorName = _context.Users
                        .Where(u => u.Id == b.CreaterId)
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (blog == null)
                return NotFound();

            return Ok(blog);
        }

        [HttpGet("creators")]
        public async Task<IActionResult> GetAllCreators()
        {
            var creators = await _context.Users
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            return Ok(creators);
        }
    }
}
