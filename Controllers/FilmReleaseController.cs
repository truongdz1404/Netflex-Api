using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netflex.Database;
using Netflex.Models.Film;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class FilmReleaseController : BaseController
    {
        private const int PAGE_SIZE = 10;
        private readonly ApplicationDbContext _context;

        public FilmReleaseController(IUnitOfWork unitOfWork,
        ApplicationDbContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public IActionResult Index(int? page, int year)
        {
            int pageNumber = page ?? 1;
            var filmQuery = _unitOfWork.Repository<Film>().Entities.Where(film => film.ProductionYear == year);

            var models = filmQuery
                .Select(film => new FilmViewModel()
                {
                    Id = film.Id,
                    Title = film.Title,
                    Poster = film.Poster,
                    Path = film.Path,
                    Trailer = film.Trailer,
                    ProductionYear = film.ProductionYear,
                    CreatedAt = film.CreatedAt
                })
                .OrderBy(f => f.CreatedAt)
                .ToPagedList(pageNumber, PAGE_SIZE);
            ViewData["Title"] = year;
            return View(models);
        }
    }
}