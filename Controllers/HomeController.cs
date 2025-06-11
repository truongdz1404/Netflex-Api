using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using Netflex.Models.Film;
using Netflex.Models.Serie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private const int ListSize = 10;
        private const int PageSize = 5;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomeData(int page = 1)
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new { u.Id, u.UserName })
                    .ToListAsync();

                var blogs = await _context.Blogs
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .Select(b => new BlogViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Content = b.Content,
                        Thumbnail = b.Thumbnail,
                        CreatedAt = b.CreatedAt,
                        CreaterId = b.CreaterId,
                        CreatorName = _context.Users
                            .Where(u => u.Id == b.CreaterId)
                            .Select(u => u.UserName)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                var totalBlogs = await _context.Blogs.CountAsync();

                var newFilms = await GetNewFilms();
                var featuredFilms = await GetFeaturedFilms();
                var series = await GetSerieFilms();

                return Ok(new
                {
                    blogs,
                    users,
                    newFilms,
                    featuredFilms,
                    series,
                    pagination = new
                    {
                        page,
                        pageSize = PageSize,
                        totalItems = totalBlogs,
                        totalPages = (int)Math.Ceiling((double)totalBlogs / PageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching home data");
                return StatusCode(500, new { message = "An error occurred while fetching home data" });
            }
        }

        [HttpGet("new-films")]
        public async Task<List<FilmViewModel>> GetNewFilms()
        {
            return await _unitOfWork.Repository<Film>().Entities
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
                .OrderByDescending(f => f.CreatedAt)
                .Take(ListSize)
                .ToListAsync();
        }

        [HttpGet("featured-films")]
        public async Task<List<FilmViewModel>> GetFeaturedFilms()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            var filmsWithRatings = await _unitOfWork.Repository<Film>().Entities
                .Where(f => f.CreatedAt >= oneMonthAgo)
                .GroupJoin(
                    _unitOfWork.Repository<Review>().Entities,
                    film => film.Id,
                    review => review.FilmId,
                    (film, reviews) => new
                    {
                        Film = film,
                        AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
                    })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.Film.CreatedAt)
                .Take(ListSize)
                .ToListAsync();

            return filmsWithRatings.Select(x => new FilmViewModel
            {
                Id = x.Film.Id,
                Title = x.Film.Title,
                Poster = x.Film.Poster,
                Path = x.Film.Path,
                Trailer = x.Film.Trailer,
                ProductionYear = x.Film.ProductionYear,
                CreatedAt = x.Film.CreatedAt
            })
            .ToList();
        }

        [HttpGet("series")]
        public async Task<List<SerieViewModel>> GetSerieFilms()
        {
            return await _unitOfWork.Repository<Serie>().Entities
                .Select(serie => new SerieViewModel
                {
                    Id = serie.Id,
                    Title = serie.Title,
                    Poster = serie.Poster,
                    ProductionYear = serie.ProductionYear,
                    CreatedAt = serie.CreatedAt
                })
                .OrderByDescending(f => f.CreatedAt)
                .Take(ListSize)
                .ToListAsync();
        }

    }
}