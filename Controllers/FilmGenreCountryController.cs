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
    public class FilmGenreCountryController : BaseController
    {
        private const int PAGE_SIZE = 10;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public FilmGenreCountryController(IUnitOfWork unitOfWork,
        ApplicationDbContext context) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IActionResult Index(int? page, Guid? genreId, Guid? countryId)
        {
            int pageNumber = page ?? 1;

            var filmGenreEntities = genreId.HasValue
                ? _context.FilmGenres.Where(fg => fg.GenreId == genreId).Select(fg => fg.FilmId)
                : null;

            var filmCountryEntities = countryId.HasValue
                ? _context.FilmCountries.Where(fc => fc.CountryId == countryId).Select(fc => fc.FilmId)
                : null;

            var filmQuery = _unitOfWork.Repository<Film>().Entities.AsQueryable();

            if (filmGenreEntities != null)
            {
                filmQuery = filmQuery.Where(film => filmGenreEntities.Contains(film.Id));
            }

            if (filmCountryEntities != null)
            {
                filmQuery = filmQuery.Where(film => filmCountryEntities.Contains(film.Id));
            }

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

            string genreName = "";
            if (genreId.HasValue)
            {
                var genre = _unitOfWork.Repository<Genre>().Entities.SingleOrDefault(g => g.Id == genreId);
                genreName = genre != null ? genre.Name : "Unknown Genre";
            }

            string countryName = "";
            if (countryId.HasValue)
            {
                var country = _unitOfWork.Repository<Country>().Entities.SingleOrDefault(c => c.Id == countryId);
                countryName = country != null ? "Phim " + country.Name : "Unknown Country";
            }

            ViewData["Title"] = $"{genreName} {countryName}".Trim();

            return View(models);
        }
    }
}