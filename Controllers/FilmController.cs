using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netflex.Database;
using Netflex.Models.Film;
using Microsoft.AspNetCore.Identity;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class FilmController : BaseController
    {
        private const int PAGE_SIZE = 10;
        private readonly ApplicationDbContext _context;
        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;

        public FilmController(IFollowRepository followRepository, IUnitOfWork unitOfWork, UserManager<User> userManager,
            ApplicationDbContext context) : base(unitOfWork)
        {
            _followRepository = followRepository;
            _userManager = userManager;
            _context = context;

        }

        public IActionResult Index(int? page, Guid? genreId, Guid? countryId, int? year)
        {
            int pageNumber = page ?? 1;
            var filmQuery = _unitOfWork.Repository<Film>().Entities.AsQueryable();

            if (genreId.HasValue)
            {
                var filmGenreEntities = _context.FilmGenres
                    .Where(fg => fg.GenreId == genreId)
                    .Select(fg => fg.FilmId);
                filmQuery = filmQuery.Where(film => filmGenreEntities.Contains(film.Id));
            }

            if (countryId.HasValue)
            {
                var filmCountryEntities = _context.FilmCountries
                    .Where(fc => fc.CountryId == countryId)
                    .Select(fc => fc.FilmId);
                filmQuery = filmQuery.Where(film => filmCountryEntities.Contains(film.Id));
            }

            if (year.HasValue)
            {
                filmQuery = filmQuery.Where(film => film.ProductionYear == year.Value);
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

            string genreName = genreId.HasValue
                ? _unitOfWork.Repository<Genre>().Entities
                    .Where(g => g.Id == genreId)
                    .Select(g => g.Name)
                    .FirstOrDefault() ?? "Unknown Genre"
                : "";

            string countryName = countryId.HasValue
                ? _unitOfWork.Repository<Country>().Entities
                    .Where(c => c.Id == countryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "Unknown Country"
                : "";

            string yearTitle = year.HasValue ? $"{year}" : "";

            ViewData["Title"] = $"{genreName} {countryName} {yearTitle}".Trim();
            if (ViewData["Title"]?.ToString() == "")
            {
                ViewData["Title"] = "Phim lẻ";
            }
            return View(models);
        }
        public async Task<IActionResult> Detail(Guid? id)
        {
            if (id == null)
                return NotFound();
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isFollowed = false;

            if (user != null)
            {
                var existingFollow = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, film.Id);
                isFollowed = existingFollow != null;
            }
            var model = new DetailFilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                About = film.About,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear,
                IsFollowed = isFollowed
            };

            var actorIds = _context.FilmActors
                .Where(fa => fa.FilmId == id)
                .Select(fa => fa.ActorId)
                .ToList();

            ViewBag.Actors = _unitOfWork.Repository<Actor>()
                .Entities
                .Where(a => actorIds.Contains(a.Id))
                .ToList();

            var genreIds = _context.FilmGenres
                .Where(fg => fg.FilmId == id)
                .Select(fg => fg.GenreId)
                .ToList();

            ViewBag.Genres = _unitOfWork.Repository<Genre>()
                .Entities
                .Where(g => genreIds.Contains(g.Id))
                .ToList();

            var relatedFilmIds = _context.FilmGenres
                .Where(fg => genreIds.Contains(fg.GenreId) && fg.FilmId != id)
                .Select(fg => fg.FilmId)
                .Distinct()
                .Take(50)
                .ToList();

            var relatedFilms = _unitOfWork.Repository<Film>()
                .Entities
                .Where(f => relatedFilmIds.Contains(f.Id))
                .OrderBy(f => f.Title)
                .Take(10)
                .ToList();

            ViewBag.RelatedFilms = relatedFilms;
            return View(model);
        }
    }
}