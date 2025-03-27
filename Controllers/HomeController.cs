using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Blog;
using System;
using System.Linq;
using System.Collections.Generic;
using Netflex.Models.Film;
using Netflex.Models.Serie;

namespace Netflex.Controllers;

public class HomeController : BaseController

{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private const int ListSize = 10;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
    IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 5;
        var users = _context.Users.ToList();
        ViewBag.Users = new SelectList(users, "Id", "UserName");

        var blogs = _context.Blogs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BlogViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                Thumbnail = b.Thumbnail,
                CreatedAt = b.CreatedAt,
                CreaterId = b.CreaterId,
                CreatorName = _context.Users.Where(u => u.Id == b.CreaterId).Select(u => u.UserName).FirstOrDefault()
            })
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
        GetFeaturedFilms();
        GetSerieFilms();
        GetNewFilms();
        return View(blogs);
    }


    public void GetNewFilms()
    {

        var models = _unitOfWork.Repository<Film>().Entities.Select(
            film => new FilmViewModel()
            {
                Id = film.Id,
                Title = film.Title,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear,
                CreatedAt = film.CreatedAt

            }
        ).OrderByDescending(f => f.CreatedAt).Take(ListSize)
        .ToList();
        ViewBag.SingleFilms = models;
    }


    public void GetFeaturedFilms()
    {
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

        var filmsWithRatings = _unitOfWork.Repository<Film>().Entities
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
            .ToList();

        var models = filmsWithRatings.Select(x => new FilmViewModel()
        {
            Id = x.Film.Id,
            Title = x.Film.Title,
            Poster = x.Film.Poster,
            Path = x.Film.Path,
            Trailer = x.Film.Trailer,
            ProductionYear = x.Film.ProductionYear,
            CreatedAt = x.Film.CreatedAt,
        })
            .ToList();

        ViewBag.FeaturedFilms = models;
    }
    public void GetSerieFilms()
    {

        var models = _unitOfWork.Repository<Serie>().Entities.Select(
            serie => new SerieViewModel()
            {
                Id = serie.Id,
                Title = serie.Title,
                Poster = serie.Poster,
                ProductionYear = serie.ProductionYear,
                CreatedAt = serie.CreatedAt
            }
        ).OrderByDescending(f => f.CreatedAt).Take(ListSize)
        .ToList();
        ViewBag.SeriesFilms = models;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        throw new BadRequestException("error");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
