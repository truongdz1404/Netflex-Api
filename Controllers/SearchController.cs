using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Film;
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
            var query = _unitOfWork.Repository<Film>().Entities;
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(f => f.Title.ToLower().Contains(title));
            }

            var models = query.Select(
                film => new DetailFilmViewModel()
                {
                    Id = film.Id,
                    Title = film.Title,
                    About = film.About,
                    Poster = film.Poster,
                    Path = film.Path,
                    Trailer = film.Trailer,
                    ProductionYear = film.ProductionYear,
                }
            ).OrderBy(f => f.ProductionYear)
            .ToPagedList(pageNumber, PAGE_SIZE);

            return View(models);
        }
    }
}