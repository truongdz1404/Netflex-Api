using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Netflex.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs
                .Select(b => new BlogViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                })
                .OrderByDescending(b => b.CreatedAt) 
                .ToList();

            return View(blogs);
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var blog = _context.Blogs
                .Where(b => b.Id == id)
                .Select(b => new BlogViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    CreatedAt = b.CreatedAt,
                    CreaterId = b.CreaterId,
                })
                .FirstOrDefault();

            if (blog == null)
                return NotFound();

            return View(blog);
        }
    }
}
