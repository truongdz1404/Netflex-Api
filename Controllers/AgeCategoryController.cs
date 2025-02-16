using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Models;

namespace Netflex.Controllers
{
    public class AgeCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AgeCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(string? SearchString, string? SortBy = "name", int page = 1, int pageSize = 10)
        {
            var repository = _unitOfWork.Repository<AgeCategory>();
            var query = repository.Entities;

            ViewData["CurrentFilter"] = SearchString;
            ViewData["SortBy"] = SortBy;
            // Tifm kiếm 
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(x => x.Name.Contains(SearchString));
            }

            // Sắp xếp
            query = SortBy switch
            {
                "name" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };

            return View(await query.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(AgeCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            AgeCategory entity = new AgeCategory() { Id = Guid.NewGuid(), Name = model.Name };
            await _unitOfWork.Repository<AgeCategory>().AddAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (category == null) return NotFound();
            AgeCategoryViewModel model = new AgeCategoryViewModel() { Name = category.Name };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, AgeCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            entity.Name = model.Name;
            await _unitOfWork.Repository<AgeCategory>().UpdateAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var category = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (category != null)
            {
                await _unitOfWork.Repository<AgeCategory>().DeleteAsync(category);
                await _unitOfWork.Save(CancellationToken.None);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
