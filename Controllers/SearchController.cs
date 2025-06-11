using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Search;
using X.PagedList.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : BaseController
    {
        private const int PAGE_SIZE = 10;

        public SearchController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Search(int? page, string? title)
        {
            try
            {
                int pageNumber = page ?? 1;
                var filmQuery = _unitOfWork.Repository<Film>().Entities.AsQueryable();
                var serieQuery = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    filmQuery = filmQuery.Where(f => f.Title.ToLower().Contains(title.ToLower()));
                    serieQuery = serieQuery.Where(s => s.Title.ToLower().Contains(title.ToLower()));
                }

                var filmModels = filmQuery.Select(film => new SearchItemViewModel
                {
                    Id = film.Id,
                    Type = "Film",
                    Title = film.Title,
                    About = film.About,
                    Poster = film.Poster,
                    ProductionYear = film.ProductionYear,
                    CreatedAt = film.CreatedAt
                });

                var serieModels = serieQuery.Select(serie => new SearchItemViewModel
                {
                    Id = serie.Id,
                    Type = "Serie",
                    Title = serie.Title,
                    About = serie.About,
                    Poster = serie.Poster,
                    ProductionYear = serie.ProductionYear,
                    CreatedAt = serie.CreatedAt
                });

                var models = await filmModels.Concat(serieModels)
         .OrderBy(m => m.CreatedAt)
         .ToListAsync();

                var pagedModels = models.ToPagedList(pageNumber, PAGE_SIZE);

                return Ok(new
                {
                    items = pagedModels,
                    pageNumber,
                    pageSize = PAGE_SIZE,
                    totalItems = pagedModels.TotalItemCount,
                    totalPages = pagedModels.PageCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while performing the search", error = ex.Message });
            }
        }
    }
}