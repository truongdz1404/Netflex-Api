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