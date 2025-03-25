using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Models.AgeCategory;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class AgeCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public const int PAGE_SIZE = 3;
        public AgeCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("/dashboard/agecategory")]
        public IActionResult Index(string? SearchString, string? SortBy = "name", int PageNumber = 1)
        {
            var repository = _unitOfWork.Repository<AgeCategory>();
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

            var result = query.Select(x => new AgeCategoryViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToPagedList(PageNumber, PAGE_SIZE);

            return View("~/Views/Dashboard/AgeCategory/Index.cshtml", result);
        }


        [Route("/dashboard/agecategory/create")]
        public IActionResult Create() => View("~/Views/Dashboard/AgeCategory/Create.cshtml");

        [HttpPost]
        [Route("/dashboard/agecategory/create")]
        public async Task<IActionResult> Create(AgeCategoryEditModel model)
        {
            if (!ModelState.IsValid) return View(model);

            AgeCategory entity = new AgeCategory() { Id = Guid.NewGuid(), Name = model.Name };
            await _unitOfWork.Repository<AgeCategory>().AddAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }

        [Route("/dashboard/agecategory/edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (category == null) return NotFound();
            AgeCategoryEditModel model = new AgeCategoryEditModel() { Name = category.Name };

            return View("~/Views/Dashboard/AgeCategory/Edit.cshtml", model);
        }

        [HttpPost]
        [Route("/dashboard/agecategory/edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, AgeCategoryEditModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            await _unitOfWork.Repository<AgeCategory>().UpdateAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        [Route("/dashboard/agecategory/delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);

            if (entity != null)
            {
                var allFilms = await _unitOfWork.Repository<Film>().GetAllAsync();
                var filmsWithCategory = allFilms.Where(f => f.AgeCategoryId == id).ToList();

                foreach (var film in filmsWithCategory)
                {
                    film.AgeCategoryId = null;
                    await _unitOfWork.Repository<Film>().UpdateAsync(film);
                }

                var allSeries = await _unitOfWork.Repository<Serie>().GetAllAsync();
                var seriesWithCategory = allSeries.Where(s => s.AgeCategoryId == id).ToList();
                
                foreach (var serie in seriesWithCategory)
                {
                    serie.AgeCategoryId = null;
                    await _unitOfWork.Repository<Serie>().UpdateAsync(serie);
                }

                await _unitOfWork.Repository<AgeCategory>().DeleteAsync(entity);
                await _unitOfWork.Save(CancellationToken.None);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
