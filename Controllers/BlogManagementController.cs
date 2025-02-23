using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Models;

namespace Netflex.Controllers
{
    public class BlogManagementController : Controller
    {
        private readonly NetflexContext context;
        public BlogManagementController(NetflexContext context) => this.context = context;
        // GET: BlogManagement
        public IActionResult Index()
        {
            var blogs = context.TblBlogs
                .Include(b => b.Creater)
                .ToList();

            return View(blogs);
        }



        // GET: BlogManagement/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = context.TblBlogs
                .Include(b => b.Creater)
                .FirstOrDefault(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }


        // GET: BlogManagement/Create
        public ActionResult Create()
        {
            ViewBag.CreaterId = new SelectList(context.TblUsers, "Id", "Id");
            ViewBag.CreaterList = context.TblUsers.ToDictionary(u => u.Id, u => u.UserName);

            return View();
        }



        // POST: BlogManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblBlog tblBlog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tblBlog.Id = Guid.NewGuid(); 
                    tblBlog.CreatedAt = DateTime.Now;

                    if (!string.IsNullOrEmpty(tblBlog.CreaterId))
                    {
                        tblBlog.Creater = context.TblUsers.FirstOrDefault(u => u.Id == tblBlog.CreaterId);
                    }

                    context.TblBlogs.Add(tblBlog);
                    context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "Error while creating blog.");
                }
            }

            ViewBag.CreaterId = new SelectList(context.TblUsers, "Id", "Id", tblBlog.CreaterId);
            ViewBag.CreaterList = context.TblUsers.ToDictionary(u => u.Id, u => u.UserName);

            return View(tblBlog);
        }



        // GET: BlogManagement/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = context.TblBlogs
                .Include(b => b.Creater)
                .FirstOrDefault(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            ViewBag.CreaterId = new SelectList(context.TblUsers, "Id", "Id", blog.CreaterId);

            ViewBag.CreaterList = context.TblUsers.ToDictionary(u => u.Id, u => u.UserName);

            return View(blog);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, TblBlog tblBlog)
        {
            if (id != tblBlog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = context.TblBlogs.Include(b => b.Creater).FirstOrDefault(b => b.Id == id);
                    if (existingBlog != null)
                    {
                        existingBlog.Title = tblBlog.Title;
                        existingBlog.Content = tblBlog.Content;
                        existingBlog.Thumbnail = tblBlog.Thumbnail;
                        existingBlog.CreatedAt = tblBlog.CreatedAt;

                        if (!string.IsNullOrEmpty(tblBlog.CreaterId))
                        {
                            existingBlog.CreaterId = tblBlog.CreaterId;
                            existingBlog.Creater = context.TblUsers.FirstOrDefault(u => u.Id == tblBlog.CreaterId);
                        }

                        context.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(tblBlog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CreaterId = new SelectList(context.TblUsers, "Id", "Id", tblBlog.CreaterId);
            return View(tblBlog);
        }


        private bool BlogExists(Guid id)
        {
            return context.TblBlogs.Any(e => e.Id == id);
        }

        // GET: BlogManagement/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = context.TblBlogs
                .Include(b => b.Creater)
                .FirstOrDefault(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }


        //POST: BlogManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var blog = context.TblBlogs.Find(id);
            if (blog != null)
            {
                context.TblBlogs.Remove(blog);
                context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
