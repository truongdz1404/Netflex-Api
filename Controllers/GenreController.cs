using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Netflex.Entities;
using Netflex.Models.Genre;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class GenreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public const int PAGE_SIZE = 10;
        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? SearchString, string? SortBy = "name", int PageNumber = 1)
        {
            var repository = _unitOfWork.Repository<Genre>();
            var query = repository.Entities;

            ViewData["CurrentFilter"] = SearchString;
            ViewData["SortBy"] = SortBy;

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(x => x.Name.ToLower().Contains(SearchString.ToLower()));
            }

            query = SortBy switch
            {
                "name" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };

            var result = query.Select(x => new GenreViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToPagedList(PageNumber, PAGE_SIZE);

            return View(result);
        }
        // 
        public IActionResult GenreDropdown()
        {
            try
            {
                var repository = _unitOfWork.Repository<Genre>();
                var genres = repository.Entities
                    .OrderBy(g => g.Name)
                    .Select(g => new GenreViewModel { Id = g.Id, Name = g.Name })
                    .ToList();

                return PartialView("_GenresPartial", genres);
            }
            catch (Exception ex)
            {
                return Content("Server Error: " + ex.Message);
            }
        }


        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(GenreEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            Genre entity = new Genre() { Id = Guid.NewGuid(), Name = model.Name };
            await _unitOfWork.Repository<Genre>().AddAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (category == null) return NotFound();
            GenreEditModel model = new GenreEditModel() { Name = category.Name };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, GenreEditModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (entity == null) return NotFound();
            entity.Name = model.Name;
            await _unitOfWork.Repository<Genre>().UpdateAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.Repository<Genre>().DeleteAsync(entity);
                await _unitOfWork.Save(CancellationToken.None);
            }
            return RedirectToAction(nameof(Index));
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var repository = _unitOfWork.Repository<Genre>();
            var genres = repository.Entities.OrderBy(x => x.Name).ToList();
            ViewData["Genres"] = genres;
            base.OnActionExecuting(context);
        }
    }
}
