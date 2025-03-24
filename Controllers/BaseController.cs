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

            var models = await _unitOfWork.Repository<Serie>().Entities.Select(
          serie => new SerieViewModel()
          {
              Id = serie.Id,
              Title = serie.Title,
              Poster = serie.Poster,
              ProductionYear = serie.ProductionYear,
              CreatedAt = serie.CreatedAt
          }
      ).OrderBy(f => f.CreatedAt).Take(10)
      .ToListAsync();
            ViewBag.StarSeries = models;

        }

    }
}
