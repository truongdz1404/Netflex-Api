using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using System;
using System.Linq;
using System.Collections.Generic;
using X.PagedList.Extensions;


namespace Netflex.Controllers
{
    public class BlogController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _context = context;
        }
        private const int PAGE_SIZE = 6;

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            var users = _context.Users.ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var blogs = _context.Blogs
                .Select(b => new BlogViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                    CreatorName = _context.Users.Where(u => u.Id == b.CreaterId).Select(u => u.UserName).FirstOrDefault()
                })
                .OrderByDescending(b => b.CreatedAt)
                .ToPagedList(pageNumber, PAGE_SIZE);

            return View(blogs);
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var users = _context.Users.ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var blog = _context.Blogs
                .Where(b => b.Id == id)
                .Select(b => new DetailBlogViewModels
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                    CreatorName = _context.Users.Where(u => u.Id == b.CreaterId).Select(u => u.UserName).FirstOrDefault()
                })
                .FirstOrDefault();

            if (blog == null)
                return NotFound();
                
            return View(blog);
        }
    }
}
