using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Film;
using X.PagedList.Extensions;
namespace Netflex.Controllers;

public class FilmController(IStorageService storage, IUnitOfWork unitOfWork)
    : Controller
{
    private readonly IStorageService _storage = storage;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private const int PAGE_SIZE = 3;
    public IActionResult Index(int? page)
    {
        int pageNumber = page ?? 1;

        var models = _unitOfWork.Repository<Film>().Entities.Select(
            film => new FilmViewModel()
            {
                Id = film.Id,
                Title = film.Title,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear
            }
        ).ToPagedList(pageNumber, PAGE_SIZE);

        return View(models);
    }
    public IActionResult Detail(Guid? id)
    {
        if (id == null)
            return NotFound();
        var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
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

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
            return NotFound();
        var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (film == null)
            return NotFound();
        await _unitOfWork.Repository<Film>().DeleteAsync(film);
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "film");
    }

    public IActionResult Edit(Guid? id)
    {
        if (id == null)
            return NotFound();
        var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (film == null)
            return NotFound();
        var model = new EditFilmViewModel
        {
            Id = film.Id,
            Title = film.Title,
            About = film.About,
            PosterUrl = film.Poster,
            FileUrl = film.Path,
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditFilmViewModel update)
    {
        if (!ModelState.IsValid)
            return View(update);

        var posterUri = update.Poster != null ? await _storage.UploadFileAsync("poster", update.Poster) : null;
        var filmUri = update.File != null ? await _storage.UploadFileAsync("film", update.File) : null;

        var newFilm = new Film()
        {
            Id = update.Id,
            Title = update.Title,
            About = update.About,
            Poster = posterUri?.ToString() ?? update.PosterUrl,
            Path = filmUri?.ToString() ?? update.FileUrl,
            Trailer = update.Trailer,
            ProductionYear = update.ProductionYear,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<Film>().UpdateAsync(newFilm);
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "film");
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFilmViewModel film)
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
            Poster = posterUri?.ToString() ?? string.Empty,
            Path = filmUri?.ToString() ?? string.Empty,
            Trailer = film.Trailer,
            ProductionYear = film.ProductionYear,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<Film>().AddAsync(newFilm);
        await _unitOfWork.Save(CancellationToken.None);

        return RedirectToAction("index", "film");
    }
}