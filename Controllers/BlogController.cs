using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var users = _context.Users.ToList();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var blogs = _context.Blogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                .ToList();

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
                .FirstOrDefault();

            if (blog == null)
                return NotFound();

            return View(blog);
        }
    }
}
