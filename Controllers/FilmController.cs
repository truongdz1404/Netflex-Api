using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models;
using Netflex.Models.Film;
using X.PagedList.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : BaseController
    {
        private const int PAGE_SIZE = 12;
        private readonly ApplicationDbContext _context;
        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;

        public FilmController(IFollowRepository followRepository, IUnitOfWork unitOfWork, UserManager<User> userManager, ApplicationDbContext context)
            : base(unitOfWork)
        {
            _followRepository = followRepository;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFilms(int? page, Guid? genreId, Guid? countryId, int? year)
        {
            try
            {
                int pageNumber = page ?? 1;
                var filmQuery = _unitOfWork.Repository<Film>().Entities.AsQueryable();

                if (genreId.HasValue)
                {
                    var filmGenreEntities = await _context.FilmGenres
                        .Where(fg => fg.GenreId == genreId)
                        .Select(fg => fg.FilmId)
                        .ToListAsync();
                    filmQuery = filmQuery.Where(film => filmGenreEntities.Contains(film.Id));
                }

                if (countryId.HasValue)
                {
                    var filmCountryEntities = await _context.FilmCountries
                        .Where(fc => fc.CountryId == countryId)
                        .Select(fc => fc.FilmId)
                        .ToListAsync();
                    filmQuery = filmQuery.Where(film => filmCountryEntities.Contains(film.Id));
                }

                if (year.HasValue)
                {
                    filmQuery = filmQuery.Where(film => film.ProductionYear == year.Value);
                }

                var models = await filmQuery
      .OrderByDescending(f => f.CreatedAt)
      .Select(film => new FilmViewModel
      {
          Id = film.Id,
          Title = film.Title,
          Poster = film.Poster,
          Path = film.Path,
          Trailer = film.Trailer,
          ProductionYear = film.ProductionYear,
          CreatedAt = film.CreatedAt
      })
      .ToListAsync(); // Note the use of ToListAsync instead of ToPagedListAsync

                var pagedModels = models.ToPagedList(pageNumber, PAGE_SIZE);

                string genreName = genreId.HasValue
                    ? await _unitOfWork.Repository<Genre>().Entities
                        .Where(g => g.Id == genreId)
                        .Select(g => g.Name)
                        .FirstOrDefaultAsync() ?? "Unknown Genre"
                    : "";

                string countryName = countryId.HasValue
                    ? await _unitOfWork.Repository<Country>().Entities
                        .Where(c => c.Id == countryId)
                        .Select(c => c.Name)
                        .FirstOrDefaultAsync() ?? "Unknown Country"
                    : "";

                string yearTitle = year.HasValue ? $"{year}" : "";
                string title = $"{genreName} {countryName} {yearTitle}".Trim();
                if (string.IsNullOrEmpty(title))
                {
                    title = "Phim lẻ";
                }

                return Ok(new
                {
                    items = pagedModels,
                    pageNumber,
                    pageSize = PAGE_SIZE,
                    totalItems = pagedModels.TotalItemCount,
                    totalPages = pagedModels.PageCount,
                    title
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching films", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFilmDetail(Guid? id)
        {
            try
            {
                if (id == null || id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Film ID" });

                var film = await _unitOfWork.Repository<Film>().Entities
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

                if (film == null)
                    return NotFound(new { message = "Film not found" });

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
                    IsFollowed = isFollowed,
                    CreatedAt = film.CreatedAt
                };

                var actorIds = await _context.FilmActors
                    .Where(fa => fa.FilmId == id)
                    .Select(fa => fa.ActorId)
                    .ToListAsync();

                var actors = await _unitOfWork.Repository<Actor>().Entities
                    .Where(a => actorIds.Contains(a.Id))
                    .ToListAsync();

                var countryIds = await _context.FilmCountries
                    .Where(fc => fc.FilmId == id)
                    .Select(fc => fc.CountryId)
                    .ToListAsync();

                var countries = await _unitOfWork.Repository<Country>().Entities
                    .Where(a => countryIds.Contains(a.Id))
                    .Select(c => c.Name)
                    .ToListAsync();

                var genreIds = await _context.FilmGenres
                    .Where(fg => fg.FilmId == id)
                    .Select(fg => fg.GenreId)
                    .ToListAsync();

                var genres = await _unitOfWork.Repository<Genre>().Entities
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();

                var relatedFilmIds = await _context.FilmGenres
                    .Where(fg => genreIds.Contains(fg.GenreId) && fg.FilmId != id)
                    .Select(fg => fg.FilmId)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                var relatedFilms = await _unitOfWork.Repository<Film>().Entities
                    .Where(f => relatedFilmIds.Contains(f.Id))
                    .OrderBy(f => f.Title)
                    .Take(10)
                    .Select(f => new FilmViewModel
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Poster = f.Poster,
                        Path = f.Path,
                        Trailer = f.Trailer,
                        ProductionYear = f.ProductionYear,
                        CreatedAt = f.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    film = model,
                    actors,
                    countries,
                    genres,
                    relatedFilms
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching film details", error = ex.Message });
            }
        }
    }
}