using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Serie;
using X.PagedList.Extensions;
namespace Netflex.Controllers;
[Authorize(Roles = "admin")]

public class SerieManagementController(IStorageService storage, IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
    : Controller
{
    private readonly IStorageService _storage = storage;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private const int PAGE_SIZE = 3;
    public IActionResult Index(int? page)
    {
        int pageNumber = page ?? 1;

        var models = _unitOfWork.Repository<Serie>().Entities.Select(
            serie => new SerieViewModel()
            {
                Id = serie.Id,
                Title = serie.Title,
                Poster = serie.Poster,
                About = serie.About,
                AgeCategoryId = serie.AgeCategoryId,
                ProductionYear = serie.ProductionYear
            }
        ).ToPagedList(pageNumber, PAGE_SIZE);

        return View(models);
    }

    public IActionResult Detail(Guid? id)
    {
        if (id == null)
            return NotFound();
        var serie = _unitOfWork.Repository<Serie>().Entities
   .Include(s => s.SerieCountries)
   .Include(s => s.SerieGenres)
   .Include(s => s.SerieActors)
   .FirstOrDefault(m => m.Id.Equals(id));
        if (serie == null)
            return NotFound();
        var model = new DetailSerieViewModel
        {
            Id = serie.Id,
            Title = serie.Title,
            About = serie.About,
            Poster = serie.Poster,
            AgeCategoryId = serie.AgeCategoryId,
            ProductionYear = serie.ProductionYear,
            CountryIds = _dbContext.SerieCountries.Where(x => x.SerieId == serie.Id).Select(x => x.CountryId).ToList(),
            GenreIds = _dbContext.SerieGenres.Where(x => x.SerieId == serie.Id).Select(x => x.GenreId).ToList(),
            ActorIds = _dbContext.SerieActors.Where(x => x.SerieId == serie.Id).Select(x => x.ActorId).ToList(),

        };

        return View(model);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
            return NotFound();
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (serie == null)
            return NotFound();
        await _unitOfWork.Repository<Serie>().DeleteAsync(serie);
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "serie");
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
            return NotFound();

        var serie = await _unitOfWork.Repository<Serie>().Entities
            .FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (serie == null)
            return NotFound();
        System.Console.WriteLine(serie.SerieCountries?.Select(c => c.CountryId).ToList());

        var model = new EditSerieModel
        {
            Id = serie.Id,
            Title = serie.Title,
            About = serie.About,
            AgeCategoryId = serie.AgeCategoryId,
            PosterUrl = serie.Poster,
            ProductionYear = serie.ProductionYear,
            CountryIds = _dbContext.SerieCountries.Where(x => x.SerieId == serie.Id).Select(x => x.CountryId).ToList(),
            GenreIds = _dbContext.SerieGenres.Where(x => x.SerieId == serie.Id).Select(x => x.GenreId).ToList(),
            ActorIds = _dbContext.SerieActors.Where(x => x.SerieId == serie.Id).Select(x => x.ActorId).ToList(),
        };
        System.Console.WriteLine(serie.AgeCategoryId);

        await PopulateViewBags(model);

        return View(model);
    }



    [HttpPost]
    public async Task<IActionResult> Edit(EditSerieModel update)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags(update);
                return View(update);
            }

            var serie = await _unitOfWork.Repository<Serie>().GetByIdAsync(update.Id);
            if (serie == null)
                return NotFound();

            serie.Title = update.Title;
            serie.About = update.About;
            serie.AgeCategoryId = update.AgeCategoryId;
            serie.ProductionYear = update.ProductionYear;
            serie.CreatedAt = DateTime.UtcNow;
            if (update.Poster != null)
            {
                var posterUri = await _storage.UploadFileAsync("poster", update.Poster);
                serie.Poster = posterUri.ToString();
            }
            else
            {
                serie.Poster = update.PosterUrl;
            }

            _dbContext.Set<SerieActor>().RemoveRange(_dbContext.Set<SerieActor>().Where(sa => sa.SerieId == serie.Id));
            _dbContext.Set<SerieGenre>().RemoveRange(_dbContext.Set<SerieGenre>().Where(sg => sg.SerieId == serie.Id));
            _dbContext.Set<SerieCountry>().RemoveRange(_dbContext.Set<SerieCountry>().Where(sc => sc.SerieId == serie.Id));

            if (update.ActorIds != null)
            {
                foreach (var actorId in update.ActorIds)
                {
                    _dbContext.Set<SerieActor>().Add(new SerieActor { SerieId = serie.Id, ActorId = actorId });
                }
            }

            if (update.GenreIds != null)
            {
                foreach (var genreId in update.GenreIds)
                {
                    _dbContext.Set<SerieGenre>().Add(new SerieGenre { SerieId = serie.Id, GenreId = genreId });
                }
            }

            if (update.CountryIds != null)
            {
                foreach (var countryId in update.CountryIds)
                {
                    _dbContext.Set<SerieCountry>().Add(new SerieCountry { SerieId = serie.Id, CountryId = countryId });
                }
            }

            await _dbContext.SaveChangesAsync();
            await _unitOfWork.Save(CancellationToken.None);

            return RedirectToAction("index", "serie");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator." + ex.Message);
            await PopulateViewBags(update);
            return View(update);
        }
    }

    private async Task PopulateViewBags(EditSerieModel model)
    {
        var actors = await _unitOfWork.Repository<Actor>().GetAllAsync() ?? new List<Actor>();
        var genres = await _unitOfWork.Repository<Genre>().GetAllAsync() ?? new List<Genre>();
        var countries = await _unitOfWork.Repository<Country>().GetAllAsync() ?? new List<Country>();

        ViewBag.Actors = new SelectList(actors, "Id", "Name", model.ActorIds);
        ViewBag.Genres = new SelectList(genres, "Id", "Name", model.GenreIds);
        ViewBag.Countries = new SelectList(countries, "Id", "Name", model.CountryIds);
        ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
        ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
        ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
        ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();

        return View(new CreateSerieModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSerieModel serie)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
            ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
            ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
            ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
            return View(serie);
        }

        var posterUri = serie.Poster != null
            ? await _storage.UploadFileAsync("poster", serie.Poster)
            : null;

        var newSerie = new Serie()
        {
            Id = Guid.NewGuid(),
            Title = serie.Title,
            About = serie.About,
            AgeCategoryId = serie.AgeCategoryId,
            Poster = posterUri?.ToString() ?? string.Empty,
            ProductionYear = serie.ProductionYear,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Serie>().AddAsync(newSerie);
        await _unitOfWork.Save(CancellationToken.None);

        if (serie.ActorIds != null)
        {
            foreach (var actorId in serie.ActorIds)
            {
                _dbContext.Set<SerieActor>().Add(new SerieActor { SerieId = newSerie.Id, ActorId = actorId });
            }
        }

        if (serie.GenreIds != null)
        {
            foreach (var genreId in serie.GenreIds)
            {
                _dbContext.Set<SerieGenre>().Add(new SerieGenre { SerieId = newSerie.Id, GenreId = genreId });
            }
        }

        if (serie.CountryIds != null)
        {
            foreach (var countryId in serie.CountryIds)
            {
                _dbContext.Set<SerieCountry>().Add(new SerieCountry { SerieId = newSerie.Id, CountryId = countryId });
            }
        }

        await _dbContext.SaveChangesAsync();
        await _unitOfWork.Save(CancellationToken.None);

        return RedirectToAction("index", "serie");
    }

}