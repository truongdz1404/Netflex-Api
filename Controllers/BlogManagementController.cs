
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using System;
using System.Linq;
using X.PagedList.Extensions;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Netflex.Controllers
{
[Authorize(Roles = "admin")]

    public class BlogManagementController(ApplicationDbContext context,
    IStorageService storage,
    IUnitOfWork unitOfWork,
    ILogger<BlogManagementController> logger)
    : Controller
    {
        private readonly IStorageService _storage = storage;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private const int PAGE_SIZE = 5;
        private readonly ApplicationDbContext _context = context;

        private readonly ILogger<BlogManagementController> _logger = logger;
        public IActionResult Index(int? page, string searchTerm, string createrName, string createrId, DateTime? createdAt, string sortOrder)
        {
            int pageNumber = page ?? 1;

            var users = _context.Users.ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var blogsQuery = _context.Blogs
                .Join(_context.Users, b => b.CreaterId, u => u.Id, (b, u) => new
                {
                    Blog = b,
                    UserName = u.UserName
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                blogsQuery = blogsQuery.Where(b => b.Blog.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(createrId))
            {
                blogsQuery = blogsQuery.Where(b => b.Blog.CreaterId == createrId);
            }

            if (!string.IsNullOrEmpty(createrName))
            {
                blogsQuery = blogsQuery.Where(b => b.UserName != null && b.UserName.Contains(createrName));
            }

            if (createdAt.HasValue)
            {
                var utcCreatedAt = createdAt.Value.Kind == DateTimeKind.Utc ? createdAt.Value : createdAt.Value.ToUniversalTime();
                blogsQuery = blogsQuery.Where(b => b.Blog.CreatedAt.Date == utcCreatedAt.Date);
            }

            switch (sortOrder)
            {
                case "title":
                    blogsQuery = blogsQuery.OrderBy(b => b.Blog.Title);
                    break;
                case "title_desc":
                    blogsQuery = blogsQuery.OrderByDescending(b => b.Blog.Title);
                    break;
                case "created_at":
                    blogsQuery = blogsQuery.OrderBy(b => b.Blog.CreatedAt);
                    break;
                case "created_at_desc":
                    blogsQuery = blogsQuery.OrderByDescending(b => b.Blog.CreatedAt);
                    break;
                case "creater_id":
                    blogsQuery = blogsQuery.OrderBy(b => b.Blog.CreaterId);
                    break;
                case "creater_id_desc":
                    blogsQuery = blogsQuery.OrderByDescending(b => b.Blog.CreaterId);
                    break;
                default:
                    blogsQuery = blogsQuery.OrderBy(b => b.Blog.Title);
                    break;
            }

            var models = blogsQuery.Select(b => new BlogViewModel()
            {
                Id = b.Blog.Id,
                Title = b.Blog.Title,
                Content = b.Blog.Content,
                Thumbnail = b.Blog.Thumbnail,
                CreatedAt = b.Blog.CreatedAt,
                CreaterId = b.Blog.CreaterId,
                CreatorName = b.UserName
            })
            .ToPagedList(pageNumber, PAGE_SIZE);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CreaterId = createrId;
            ViewBag.CreatedAt = createdAt;
            ViewBag.SortOrder = sortOrder;

            return View(models);
        }

        public IActionResult ExportToExcel()
        {
            var blogsQuery = _unitOfWork.Repository<Blog>().Entities.AsQueryable();

            var blogsData = blogsQuery.Select(b => new
            {
                b.Id,
                b.Title,
                b.Content,
                b.CreaterId,
                b.CreatedAt
            }).ToList();

            // Create the Excel file using EPPlus
            using (var package = new ExcelPackage())
            {
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
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var b = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (b == null)
                return NotFound();
            var model = new DetailBlogViewModels
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                Thumbnail = b.Thumbnail,
                CreatedAt = b.CreatedAt,
                CreaterId = b.CreaterId,
            };

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogViewModels blogViewModel)
        {
            if (!ModelState.IsValid) {
                ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName");
                return View(blogViewModel);
            }
                

            var thumbnailUri = blogViewModel.Thumbnail != null ? await _storage.UploadFileAsync("thumbnail", blogViewModel.Thumbnail) : null;

            if (blogViewModel.Thumbnail == null)
            {
                _logger.LogWarning("No thumbnail uploaded.");
            }
            else
            {
                _logger.LogInformation("Thumbnail uploaded: {FileName}", blogViewModel.Thumbnail.FileName);
            }
            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                Title = blogViewModel.Title,
                Content = blogViewModel.Content,
                Thumbnail = thumbnailUri?.ToString() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                CreaterId = blogViewModel.CreaterId
            };

            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blogViewModel.CreaterId);
            await _unitOfWork.Repository<Blog>().AddAsync(blog);
            await _unitOfWork.Save(CancellationToken.None);

            return RedirectToAction("index", "blogmanagement");
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
                return NotFound();
            var blog = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (blog == null)
                return NotFound();

            var blogViewModel = new EditBlogViewModels
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                ThumbnailUrl = blog.Thumbnail,
                CreatedAt = DateTime.UtcNow,
                CreaterId = blog.CreaterId
            };
            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blog.CreaterId);
            return View(blogViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBlogViewModels blogViewModel)
        {
            if (!ModelState.IsValid)
                return View(blogViewModel);

            var thumbnailUri = blogViewModel.Thumbnail != null ? await _storage.UploadFileAsync("thumbnail", blogViewModel.Thumbnail) : null;
            var newBlog = new Blog()
            {

                Id = blogViewModel.Id,
                Title = blogViewModel.Title,
                Content = blogViewModel.Content,
                Thumbnail = thumbnailUri?.ToString() ?? blogViewModel.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow,
                CreaterId = blogViewModel.CreaterId
            };
            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blogViewModel.CreaterId);
            await _unitOfWork.Repository<Blog>().UpdateAsync(newBlog);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction("index", "blogmanagement");
        }


        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();
            var b = _unitOfWork.Repository<Blog>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (b == null)
                return NotFound();
            await _unitOfWork.Repository<Blog>().DeleteAsync(b);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction("index", "blogmanagement");
        }
    }
}
