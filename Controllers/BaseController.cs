using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Models.Film;
using Netflex.Models.Serie;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilterOptions()
        {
            var genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
            var countries = await _unitOfWork.Repository<Country>().GetAllAsync();
            var filmYears = await _unitOfWork.Repository<Film>().Entities
                .Select(f => f.ProductionYear)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();

            return Ok(new
            {
                genres,
                countries,
                filmYears
            });
        }

        [HttpGet("star-series")]
        public async Task<IActionResult> GetStarSeries()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-4);

            var seriesQuery = _unitOfWork.Repository<Serie>()
                .Entities
                .Where(s => s.CreatedAt >= oneMonthAgo);

            var seriesWithRatings = await seriesQuery
                .GroupJoin(
                    _unitOfWork.Repository<Review>().Entities,
                    serie => serie.Id,
                    review => review.SerieId,
                    (serie, reviews) => new
                    {
                        Serie = serie,
                        AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
                    })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.Serie.CreatedAt)
                .Take(10)
                .ToListAsync();

            var series = seriesWithRatings.Select(x => new SerieViewModel()
            {
                Id = x.Serie.Id,
                Title = x.Serie.Title,
                Poster = x.Serie.Poster,
                ProductionYear = x.Serie.ProductionYear,
                CreatedAt = x.Serie.CreatedAt,
                Rating = x.AverageRating
            }).ToList();

            return Ok(series);
        }
    }
}
