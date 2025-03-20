using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

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
            await next(); 
        }
    }
}
