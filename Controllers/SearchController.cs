using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Film;
using Netflex.Models.Search;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class SearchController : BaseController
    {
        private const int PAGE_SIZE = 10;

        public SearchController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IActionResult Index(int? page, string? title)
        {
            int pageNumber = page ?? 1;
            var filmQuery = _unitOfWork.Repository<Film>().Entities.AsQueryable();
            var serieQuery = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                filmQuery = filmQuery.Where(f => f.Title.ToLower().Contains(title.ToLower()));
                serieQuery = serieQuery.Where(s => s.Title.ToLower().Contains(title.ToLower()));
            }

            var filmModels = filmQuery.Select(film => new SearchItemViewModel()
            {
                Id = film.Id,
                Type = "Film",
                Title = film.Title,
                About = film.About,
                Poster = film.Poster,
                ProductionYear = film.ProductionYear,
                CreatedAt = film.CreatedAt
            });

            var serieModels = serieQuery.Select(serie => new SearchItemViewModel()
            {
                Id = serie.Id,
                Type = "Serie",
                Title = serie.Title,
                About = serie.About,
                Poster = serie.Poster,
                ProductionYear = serie.ProductionYear,
                CreatedAt = serie.CreatedAt
            });

            var models = filmModels.Concat(serieModels)
                .OrderBy(m => m.CreatedAt)
                .ToPagedList(pageNumber, PAGE_SIZE);

            return View(models);
        }
    }
}