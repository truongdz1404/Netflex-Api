using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using System;
using System.Linq;
using X.PagedList;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class BlogManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BlogManagementController> _logger;

        public BlogManagementController(ApplicationDbContext context,
            IStorageService storage,
            IUnitOfWork unitOfWork,
            ILogger<BlogManagementController> logger)
        {
            _context = context;
            _storage = storage;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll(int? page = 1, string? searchTerm = null, string? createrName = null,
            string? createrId = null, DateTime? createdAt = null, string? sortOrder = null)
        {
            int pageNumber = page ?? 1;
            var blogsQuery = _context.Blogs
                .Join(_context.Users, b => b.CreaterId, u => u.Id, (b, u) => new { Blog = b, UserName = u.UserName })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                blogsQuery = blogsQuery.Where(b => b.Blog.Title.Contains(searchTerm));
            if (!string.IsNullOrEmpty(createrId))
                blogsQuery = blogsQuery.Where(b => b.Blog.CreaterId == createrId);
            if (!string.IsNullOrEmpty(createrName))
                blogsQuery = blogsQuery.Where(b => b.UserName.Contains(createrName));
            if (createdAt.HasValue)
                blogsQuery = blogsQuery.Where(b => b.Blog.CreatedAt.Date == createdAt.Value.Date);

            blogsQuery = sortOrder switch
            {
                "title" => blogsQuery.OrderBy(b => b.Blog.Title),
                "title_desc" => blogsQuery.OrderByDescending(b => b.Blog.Title),
                "created_at" => blogsQuery.OrderBy(b => b.Blog.CreatedAt),
                "created_at_desc" => blogsQuery.OrderByDescending(b => b.Blog.CreatedAt),
                "creater_id" => blogsQuery.OrderBy(b => b.Blog.CreaterId),
                "creater_id_desc" => blogsQuery.OrderByDescending(b => b.Blog.CreaterId),
                _ => blogsQuery.OrderBy(b => b.Blog.Title)
            };

            var result = blogsQuery.Select(b => new BlogViewModel
            {
                Id = b.Blog.Id,
                Title = b.Blog.Title,
                Content = b.Blog.Content,
                Thumbnail = b.Blog.Thumbnail,
                CreatedAt = b.Blog.CreatedAt,
                CreaterId = b.Blog.CreaterId,
                CreatorName = b.UserName
            }).ToPagedList(pageNumber, 6);

            return Ok(result);
        }

        [HttpGet("export")]
        public IActionResult ExportToExcel()
        {
            var blogsData = _unitOfWork.Repository<Blog>().Entities.Select(b => new
            {
                b.Id,
                b.Title,
                b.Content,
                b.CreaterId,
                b.CreatedAt
            }).ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Blogs");
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Title";
            worksheet.Cells[1, 3].Value = "Content";
            worksheet.Cells[1, 4].Value = "Creator ID";
            worksheet.Cells[1, 5].Value = "Created At";

            for (int i = 0; i < blogsData.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = blogsData[i].Id;
                worksheet.Cells[i + 2, 2].Value = blogsData[i].Title;
                worksheet.Cells[i + 2, 3].Value = blogsData[i].Content;
                worksheet.Cells[i + 2, 4].Value = blogsData[i].CreaterId;
                worksheet.Cells[i + 2, 5].Value = blogsData[i].CreatedAt;
            }

            var fileContents = package.GetAsByteArray();
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Blogs.xlsx");
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var b = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (b == null) return NotFound();

            var model = new DetailBlogViewModels
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                Thumbnail = b.Thumbnail,
                CreatedAt = b.CreatedAt,
                CreaterId = b.CreaterId
            };
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBlogViewModels blogViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var thumbnailUri = blogViewModel.Thumbnail != null
                ? await _storage.UploadFileAsync("thumbnail", blogViewModel.Thumbnail)
                : null;

            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                Title = blogViewModel.Title,
                Content = blogViewModel.Content,
                Thumbnail = thumbnailUri?.ToString() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                CreaterId = blogViewModel.CreaterId
            };

            await _unitOfWork.Repository<Blog>().AddAsync(blog);
            await _unitOfWork.Save(CancellationToken.None);

            return Ok(blog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EditBlogViewModels blogViewModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var blog = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();

            var thumbnailUri = blogViewModel.Thumbnail != null
                ? await _storage.UploadFileAsync("thumbnail", blogViewModel.Thumbnail)
                : null;

            blog.Title = blogViewModel.Title;
            blog.Content = blogViewModel.Content;
            blog.Thumbnail = thumbnailUri?.ToString() ?? blogViewModel.ThumbnailUrl;
            blog.CreaterId = blogViewModel.CreaterId;
            blog.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Blog>().UpdateAsync(blog);
            await _unitOfWork.Save(CancellationToken.None);

            return Ok(blog);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var blog = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (blog == null) return NotFound();

            await _unitOfWork.Repository<Blog>().DeleteAsync(blog);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }
    }
}
