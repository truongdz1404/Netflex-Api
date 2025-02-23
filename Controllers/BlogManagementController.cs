using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models;
using System;
using System.Linq;

namespace Netflex.Controllers
{
    public class BlogManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogManagementController(ApplicationDbContext context)
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
                }).ToList();

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
                }).FirstOrDefault();

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        public IActionResult Create()
        {
            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogViewModel blogViewModel)
        {
            if (ModelState.IsValid)
            {
                var blog = new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = blogViewModel.Title,
                    Content = blogViewModel.Content,
                    Thumbnail = blogViewModel.Thumbnail,
                    CreatedAt = DateTime.Now,
                    CreaterId = blogViewModel.CreaterId
                };

                _context.Blogs.Add(blog);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blogViewModel.CreaterId);
            return View(blogViewModel);
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var blog = _context.Blogs.Find(id);
            if (blog == null)
                return NotFound();

            var blogViewModel = new BlogViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Thumbnail = blog.Thumbnail,
                CreatedAt = blog.CreatedAt,
                CreaterId = blog.CreaterId
            };

            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blog.CreaterId);
            return View(blogViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, BlogViewModel blogViewModel)
        {
            if (id != blogViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var blog = _context.Blogs.Find(id);
                if (blog == null)
                    return NotFound();

                blog.Title = blogViewModel.Title;
                blog.Content = blogViewModel.Content;
                blog.Thumbnail = blogViewModel.Thumbnail;
                blog.CreaterId = blogViewModel.CreaterId;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CreaterId = new SelectList(_context.Users, "Id", "UserName", blogViewModel.CreaterId);
            return View(blogViewModel);
        }

        public IActionResult Delete(Guid? id)
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
                }).FirstOrDefault();

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
