using Microsoft.AspNetCore.Mvc;
using Netflex.Database;
using Netflex.Models.Film;
using X.PagedList.Extensions;
namespace Netflex.Controllers;

public class FilmController(IStorageService storage,
    ApplicationDbContext context)
    : Controller
{
    private readonly IStorageService _storage = storage;
    private readonly ApplicationDbContext _context = context;
    private const int PAGE_SIZE = 3;
    public IActionResult Index(int? page)
    {
        int pageNumber = page ?? 1;

        var models = _context.Films.Select(film => new FilmViewModel()
        {
            Id = film.Id,
            Title = film.Title,
            Poster = film.Poster,
            Path = film.Path,
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear
        }).ToPagedList(pageNumber, PAGE_SIZE);

        return View(models);
    }
    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Detail(Guid? id)
    {
        if (id == null)
            return NotFound();
        var film = _context.Films.FirstOrDefault(m => m.Id.Equals(id));
        if (film == null)
            return NotFound();
        var model = new DetailFilmViewModel
        {
            Id = film.Id,
            Title = film.Title,
            About = film.About,
            Poster = film.Poster,
            Path = film.Path,
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear
        };

        return View(model);
    }

    public IActionResult Edit(Guid? id)
    {
        if (id == null)
            return NotFound();
        var film = _context.Films.FirstOrDefault(m => m.Id.Equals(id));
        if (film == null)
            return NotFound();
        var model = new DetailFilmViewModel
        {
            Id = film.Id,
            Title = film.Title,
            About = film.About,
            Poster = film.Poster,
            Path = film.Path,
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear
        };

        return View(model);
    }
    [HttpPost]
    public IActionResult Edit(EditFilmModel film)
    {
        // if (!ModelState.IsValid)
        //     return View(film);

        // var posterUri = film.Poster != null ? await _storage.UploadFileAsync("poster", film.Poster) : null;
        // var filmUri = film.File != null ? await _storage.UploadFileAsync("film", film.File) : null;

        // var newFilm = new Film()
        // {
        //     Id = Guid.NewGuid(),
        //     Title = film.Title,
        //     About = film.About,
        //     Poster = posterUri?.ToString(),
        //     Path = filmUri?.ToString(),
        //     Trailer = film.Trailer,
        //     ProductionYear = film.ProductionYear,
        //     CreatedAt = DateTime.Now
        // };
        // _context.Films.Add(newFilm);
        // _context.SaveChanges();

        return RedirectToAction("index", "film");
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFilmModel film)
    {
        if (!ModelState.IsValid)
            return View(film);

        var posterUri = film.Poster != null ? await _storage.UploadFileAsync("poster", film.Poster) : null;
        var filmUri = film.File != null ? await _storage.UploadFileAsync("film", film.File) : null;

        var newFilm = new Film()
        {
            Id = Guid.NewGuid(),
            Title = film.Title,
            About = film.About,
            Poster = posterUri?.ToString(),
            Path = filmUri?.ToString(),
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear,
            CreatedAt = DateTime.Now
        };
        _context.Films.Add(newFilm);
        _context.SaveChanges();

        return RedirectToAction("index", "film");
    }
}