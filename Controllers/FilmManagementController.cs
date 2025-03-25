using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Netflex.Database;
using Netflex.Models.Film;
using X.PagedList.Extensions;
namespace Netflex.Controllers;

[Authorize(Roles = "admin")]
public class FilmManagementController(IStorageService storage, IUnitOfWork unitOfWork, ApplicationDbContext context)
    : Controller
{
    private readonly IStorageService _storage = storage;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationDbContext _context = context;
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

    public async Task<IActionResult> Edit(Guid? id)
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
            ProductionYear = film.ProductionYear,
            CountryIds = _context.FilmCountries.Where(x => x.FilmId == film.Id).Select(x => x.CountryId).ToList(),
            GenreIds = _context.FilmGenres.Where(x => x.FilmId == film.Id).Select(x => x.GenreId).ToList(),
            ActorIds = _context.FilmActors.Where(x => x.FilmId == film.Id).Select(x => x.ActorId).ToList(),
        };
        await PopulateViewBags(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditFilmViewModel update)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags(update);
            return View(update);
        }
        var film = await _unitOfWork.Repository<Film>().GetByIdAsync(update.Id);
        if (film == null)
            return NotFound();
        var filmUri = update.File != null ? await _storage.UploadFileAsync("film", update.File) : null;
        film.Title = update.Title;
        film.About = update.About;
        film.AgeCategoryId = update.AgeCategoryId;
        film.ProductionYear = update.ProductionYear;
        film.CreatedAt = DateTime.UtcNow;
        film.Path = filmUri?.ToString() ?? update.FileUrl;
        film.Trailer = update.Trailer;
        Uri posterUri;
        if (update.Poster != null)
        {
            posterUri = await _storage.UploadFileAsync("poster", update.Poster);
            film.Poster = posterUri.ToString();
        }
        else
        {
            film.Poster = update.PosterUrl;
        }
        _context.Set<FilmActor>().RemoveRange(_context.Set<FilmActor>().Where(sa => sa.FilmId == film.Id));
        _context.Set<FilmGenre>().RemoveRange(_context.Set<FilmGenre>().Where(sg => sg.FilmId == film.Id));
        _context.Set<FilmCountry>().RemoveRange(_context.Set<FilmCountry>().Where(sc => sc.FilmId == film.Id));

        if (update.ActorIds != null)
        {
            foreach (var actorId in update.ActorIds)
            {
                _context.Set<FilmActor>().Add(new FilmActor { FilmId = film.Id, ActorId = actorId });
            }
        }

        if (update.GenreIds != null)
        {
            foreach (var genreId in update.GenreIds)
            {
                _context.Set<FilmGenre>().Add(new FilmGenre { FilmId = film.Id, GenreId = genreId });
            }
        }

        if (update.CountryIds != null)
        {
            foreach (var countryId in update.CountryIds)
            {
                _context.Set<FilmCountry>().Add(new FilmCountry { FilmId = film.Id, CountryId = countryId });
            }
        }

        await _context.SaveChangesAsync();
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "film");
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
        ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
        ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
        ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFilmViewModel film)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
            ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
            ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
            ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
            return View(film);
        }

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
        if (film.ActorIds != null)
        {
            foreach (var actorId in film.ActorIds)
            {
                _context.Set<FilmActor>().Add(new FilmActor { FilmId = newFilm.Id, ActorId = actorId });
            }
        }

        if (film.GenreIds != null)
        {
            foreach (var genreId in film.GenreIds)
            {
                _context.Set<FilmGenre>().Add(new FilmGenre { FilmId = newFilm.Id, GenreId = genreId });
            }
        }

        if (film.CountryIds != null)
        {
            foreach (var countryId in film.CountryIds)
            {
                _context.Set<FilmCountry>().Add(new FilmCountry { FilmId = newFilm.Id, CountryId = countryId });
            }
        }

        await _context.SaveChangesAsync();
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "film");
    }

    private async Task PopulateViewBags(EditFilmViewModel model)
    {
        var actors = await _unitOfWork.Repository<Actor>().GetAllAsync() ?? new List<Actor>();
        var genres = await _unitOfWork.Repository<Genre>().GetAllAsync() ?? new List<Genre>();
        var countries = await _unitOfWork.Repository<Country>().GetAllAsync() ?? new List<Country>();

        ViewBag.Actors = new SelectList(actors, "Id", "Name", model.ActorIds);
        ViewBag.Genres = new SelectList(genres, "Id", "Name", model.GenreIds);
        ViewBag.Countries = new SelectList(countries, "Id", "Name", model.CountryIds);
        ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
    }
}