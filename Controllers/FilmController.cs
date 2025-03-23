using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netflex.Models.Film;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class FilmController : BaseController
    {
        private const int PAGE_SIZE = 10;
        public FilmController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;

            var models = _unitOfWork.Repository<Film>().Entities.Select(
                film => new FilmViewModel()
                {
                    Id = film.Id,
                    Title = film.Title,
                    Poster = film.Poster,
                    Path = film.Path,
                    Trailer = film.Trailer,
                    ProductionYear = film.ProductionYear,
                    CreatedAt = film.CreatedAt

                }
            ).OrderBy(f => f.CreatedAt)
            .ToPagedList(pageNumber, PAGE_SIZE);

            return View(models);
        }
        public IActionResult Detail(Guid? id)
        {
            if (id == null)
                return NotFound();
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
                return NotFound();
            var model = new DetailFilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                About = film.About,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear
            };

            return View(model);
        }
    }
}