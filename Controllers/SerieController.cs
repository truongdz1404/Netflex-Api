using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models;
using Netflex.Models.Serie;
using X.PagedList.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerieController : BaseController
    {
        private const int PAGE_SIZE = 12;
        private readonly ApplicationDbContext _context;
        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;

        public SerieController(IFollowRepository followRepository, IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<User> userManager) : base(unitOfWork)
        {
            _context = context;
            _followRepository = followRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSeries(int? page)
        {
            try
            {
                int pageNumber = page ?? 1;

                var series = await _unitOfWork.Repository<Serie>().Entities
      .OrderByDescending(m => m.CreatedAt)
      .ToListAsync();

                var models = series.ToPagedList(pageNumber, PAGE_SIZE)
                    .Select(serie => new SerieViewModel
                    {
                        Id = serie.Id,
                        Title = serie.Title,
                        Poster = serie.Poster,
                        About = serie.About,
                        AgeCategoryId = serie.AgeCategoryId,
                        ProductionYear = serie.ProductionYear
                    });

                return Ok(new
                {
                    items = models,
                    pageNumber,
                    pageSize = PAGE_SIZE,
                    totalItems = models.TotalItemCount,
                    totalPages = models.PageCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching series", error = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetSeriesFilter(int? page, Guid? genreId, Guid? countryId, int? year, string? keyword)
        {
            try
            {
                int pageNumber = page ?? 1;
                var serieQuery = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

                if (genreId.HasValue)
                {
                    var serieGenreEntities = await _context.SerieGenres
                        .Where(sg => sg.GenreId == genreId)
                        .Select(sg => sg.SerieId)
                        .ToListAsync();
                    serieQuery = serieQuery.Where(serie => serieGenreEntities.Contains(serie.Id));
                }

                if (countryId.HasValue)
                {
                    var serieCountryEntities = await _context.SerieCountries
                        .Where(sc => sc.CountryId == countryId)
                        .Select(sc => sc.SerieId)
                        .ToListAsync();
                    serieQuery = serieQuery.Where(serie => serieCountryEntities.Contains(serie.Id));
                }

                if (year.HasValue)
                {
                    serieQuery = serieQuery.Where(serie => serie.ProductionYear == year.Value);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    serieQuery = serieQuery.Where(serie => serie.Title.ToLower().Contains(keyword.ToLower()));
                }

                var models = await serieQuery
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(serie => new
                    {
                        Id = serie.Id,
                        Title = serie.Title,
                        Poster = serie.Poster,
                        ProductionYear = serie.ProductionYear,
                        CreatedAt = serie.CreatedAt
                    })
                    .ToListAsync();

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
                    title = "Series";
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
                return StatusCode(500, new { message = "An error occurred while fetching series", error = ex.ToString() });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSerieDetail(Guid? id)
        {
            try
            {
                if (id == null || id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Series ID" });

                var serie = await _unitOfWork.Repository<Serie>().Entities
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

                if (serie == null)
                    return NotFound(new { message = "Series not found" });

                var user = await _userManager.GetUserAsync(User);
                bool isFollowed = false;

                if (user != null)
                {
                    var existingFollow = await _followRepository.GetByUserIdAndSerieIdAsync(user.Id, serie.Id);
                    isFollowed = existingFollow != null;
                }

                var model = new DetailSerieViewModel
                {
                    Id = serie.Id,
                    Title = serie.Title,
                    About = serie.About,
                    Poster = serie.Poster,
                    AgeCategoryId = serie.AgeCategoryId,
                    ProductionYear = serie.ProductionYear,
                    CountryIds = await _context.SerieCountries
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.CountryId)
                        .ToListAsync(),
                    GenreIds = await _context.SerieGenres
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.GenreId)
                        .ToListAsync(),
                    ActorIds = await _context.SerieActors
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.ActorId)
                        .ToListAsync(),
                    IsFollowed = isFollowed,
                    CreatedAt = serie.CreatedAt
                };

                var episodes = await _unitOfWork.Repository<Episode>().Entities
                    .Where(e => e.SerieId == id)
                    .ToListAsync();

                var countryIds = await _context.SerieCountries
                    .Where(fc => fc.SerieId == id)
                    .Select(fc => fc.CountryId)
                    .ToListAsync();

                var countries = await _unitOfWork.Repository<Country>().Entities
                    .Where(a => countryIds.Contains(a.Id))
                    .Select(c => c.Name)
                    .ToListAsync();

                var actorIds = await _context.SerieActors
                    .Where(fa => fa.SerieId == id)
                    .Select(fa => fa.ActorId)
                    .ToListAsync();

                var actors = await _unitOfWork.Repository<Actor>().Entities
                    .Where(a => actorIds.Contains(a.Id))
                    .ToListAsync();

                var genreIds = await _context.SerieGenres
                    .Where(fg => fg.SerieId == id)
                    .Select(fg => fg.GenreId)
                    .ToListAsync();

                var genres = await _unitOfWork.Repository<Genre>().Entities
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();

                var relatedSerieIds = await _context.SerieGenres
                    .Where(fg => genreIds.Contains(fg.GenreId) && fg.SerieId != id)
                    .Select(fg => fg.SerieId)
                    .Distinct()
                    .Take(50)
                    .ToListAsync();

                var relatedSeries = await _unitOfWork.Repository<Serie>().Entities
                    .Where(f => relatedSerieIds.Contains(f.Id))
                    .OrderBy(f => f.Title)
                    .Take(10)
                    .Select(s => new SerieViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Poster = s.Poster,
                        About = s.About,
                        AgeCategoryId = s.AgeCategoryId,
                        ProductionYear = s.ProductionYear
                    })
                    .ToListAsync();

                return Ok(new
                {
                    serie = model,
                    episodes,
                    countries,
                    actors,
                    genres,
                    relatedSeries
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching series details", error = ex.Message });
            }
        }
    }
}