using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult> GetComments(int page = 1, int pageSize = 10, string sort = "desc")
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page và pageSize phải > 0");

            IQueryable<Comment> query = _context.Comments.Include(x => x.User);

            sort = sort.ToLower();
            query = sort switch
            {
                "asc" => query.OrderBy(c => c.CreatedAt),
                "desc" => query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var hasMore = page < totalPages;

            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dto = comments.Select(x => new CommentDto
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                FilmId = x.Id,
                ModifiedAt = x.ModifiedAt,
                SeriesId = x.SeriesId,
                User = x.User,
            });

            return Ok(new
            {
                comments = dto,
                currentPage = page,
                pageSize,
                sort,
                hasMore,
                totalItems,
            });
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            var dto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                FilmId = comment.Id,
                ModifiedAt = comment.ModifiedAt,
                SeriesId = comment.SeriesId,
                User = comment.User,
            };

            return Ok(dto);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, EditCommentDto dto)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            comment.Content = dto.Content;
            comment.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<IActionResult> PostComment(CreateCommentDto dto)
        {
            try
            {
                var newComment = new Comment
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Content = dto.Content,
                    FilmId = dto.FilmId.HasValue ? dto.FilmId.Value : null,
                    UserId = dto.UserId,
                    SeriesId = dto.SeriesId.HasValue ? dto.SeriesId.Value : null,
                };

                await _context.Comments.AddAsync(newComment);
                await _context.SaveChangesAsync();

                return Created();
            }

            catch (Exception)
            {
                return BadRequest("Lỗi không xác định.");
            }
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class CommentDto
        {
            public Guid Id { get; set; }
            public string? Content { get; set; }

            public User User { get; set; }

            [ForeignKey(nameof(Film))]
            public Guid? FilmId { get; set; }

            public Guid? SeriesId { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }

        public class UserCommentDto
        {

        }

        public class CreateCommentDto
        {
            [Required]
            public string UserId { get; set; }
            [Required]
            public string Content { get; set; } = string.Empty;

            public Guid? FilmId { get; set; }
            public Guid? SeriesId { get; set; }
        }

        public class EditCommentDto
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
