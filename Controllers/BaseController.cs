using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Netflex.Models.Film;
using Netflex.Models.Serie;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
            ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
            ViewBag.FilmYearReleases = await _unitOfWork.Repository<Film>().Entities
                .Select(f => f.ProductionYear)
                .Distinct()
                .OrderBy(f => f)
                .ToListAsync();
            await GetStarSeries();
            await next();
        }

        public async Task GetStarSeries()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

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

            ViewBag.StarSeries = series;
        }
    }
    }
