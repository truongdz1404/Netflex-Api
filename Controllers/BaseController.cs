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
            var series = await _unitOfWork.Repository<Serie>()
                .Entities
                .OrderBy(f => f.CreatedAt)
                .Take(10)
                .Select(serie => new SerieViewModel()
                {
                    Id = serie.Id,
                    Title = serie.Title,
                    Poster = serie.Poster,
                    ProductionYear = serie.ProductionYear,
                    CreatedAt = serie.CreatedAt,
                })
                .ToListAsync();

            // var serieIds = series.Select(s => s.Id).ToList();
            // var reviews = await _unitOfWork.Repository<Review>()
            // .Entities
            // .Where(r => r.SerieId.HasValue && serieIds.Contains(r.SerieId.Value))
            // .GroupBy(r => r.SerieId != null ? r.SerieId.Value : Guid.Empty)
            // .Select(g => new
            // {
            //     SerieId = g.Key,
            //     AverageRating = g.Average(r => r.Rating)
            // })
            // .ToListAsync();

            // foreach (var serie in series)
            // {
            //     var review = reviews.FirstOrDefault(r => r.SerieId == serie.Id);
            //     serie.Rating = review != null ? review.AverageRating : 1;
            // }

            ViewBag.StarSeries = series;
        }

    }
}
