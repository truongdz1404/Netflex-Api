using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Serie;
using OfficeOpenXml;
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

    [Route("/dashboard/serie")]
    public IActionResult Index(string? searchTerm, int? productionYear, string? sortOrder, int? page, bool export = false)
    {
        int pageNumber = page ?? 1;

        var query = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(s => s.Title.ToLower().Contains(searchTerm.ToLower()));
        }

        if (productionYear.HasValue)
        {
            query = query.Where(s => s.ProductionYear == productionYear.Value);
        }

        query = sortOrder switch
        {
            "title" => query.OrderBy(s => s.Title),
            "title_desc" => query.OrderByDescending(s => s.Title),
            "production_year" => query.OrderBy(s => s.ProductionYear),
            "production_year_desc" => query.OrderByDescending(s => s.ProductionYear),
            _ => query.OrderBy(s => s.Title)
        };

        if (export)
        {
            return ExportToExcel(searchTerm, productionYear, sortOrder);
        }

        var models = query.Select(serie => new SerieViewModel()
        {
            Id = serie.Id,
            Title = serie.Title,
            Poster = serie.Poster,
            About = serie.About,
            AgeCategoryId = serie.AgeCategoryId,
            ProductionYear = serie.ProductionYear
        }).ToPagedList(pageNumber, PAGE_SIZE);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.ProductionYear = productionYear;
        ViewBag.SortOrder = sortOrder;

        return View("~/Views/Dashboard/Serie/Index.cshtml", models);
    }

    public IActionResult ExportToExcel(string? searchTerm, int? productionYear, string? sortOrder)
    {
        var query = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(f => f.Title.Contains(searchTerm));
        }

        if (productionYear.HasValue)
        {
            query = query.Where(f => f.ProductionYear == productionYear);
        }

        query = sortOrder switch
        {
            "title" => query.OrderBy(f => f.Title),
            "title_desc" => query.OrderByDescending(f => f.Title),
            "production_year" => query.OrderBy(f => f.ProductionYear),
            "production_year_desc" => query.OrderByDescending(f => f.ProductionYear),
            _ => query.OrderBy(f => f.Title)
        };

        var films = query.Select(f => new SerieViewModel
        {
            Id = f.Id,
            Title = f.Title,
            Poster = f.Poster,
            ProductionYear = f.ProductionYear
        }).ToList();

        if (!films.Any())
        {
            return Content("No films found for export.", "text/plain");
        }

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Films");

        worksheet.Cells[1, 1].Value = "Id";
        worksheet.Cells[1, 2].Value = "Title";
        worksheet.Cells[1, 3].Value = "Production Year";
        worksheet.Cells[1, 4].Value = "Trailer Link";
        Console.WriteLine($"Exporting {films.Count} films");

        for (int i = 0; i < films.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = films[i].Title;
            worksheet.Cells[i + 2, 2].Value = films[i].ProductionYear;
        }
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Series-{Guid.NewGuid()}.xlsx");
    }


    [Route("/dashboard/serie/detail/{id}")]
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

        return View("~/Views/Dashboard/Serie/Detail.cshtml", model);
    }

    [HttpDelete]
    [Route("/dashboard/serie/delete/{id}")]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
            return NotFound();
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (serie == null)
            return NotFound();
        var follows = _unitOfWork.Repository<Follow>().Entities.Where(f => f.SerieId == serie.Id).ToList();
        foreach (var follow in follows)
        {
            await _unitOfWork.Repository<Follow>().DeleteAsync(follow);
        }
        var ratings = _unitOfWork.Repository<Review>().Entities.Where(f => f.SerieId == serie.Id).ToList();
        foreach (var rating in ratings)
        {
            await _unitOfWork.Repository<Review>().DeleteAsync(rating);
        }
        await _unitOfWork.Repository<Serie>().DeleteAsync(serie);
        await _unitOfWork.Save(CancellationToken.None);
        return RedirectToAction("index", "seriemanagement");
    }

    [Route("/dashboard/serie/edit/{id}")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
            return NotFound();

        var serie = await _unitOfWork.Repository<Serie>().Entities
            .FirstOrDefaultAsync(m => m.Id.Equals(id));

        if (serie == null)
            return NotFound();

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

        await PopulateViewBags(model);

        return View("~/Views/Dashboard/Serie/Edit.cshtml", model);
    }



    [HttpPost]
    [Route("/dashboard/serie/edit/{id}")]
    public async Task<IActionResult> Edit(EditSerieModel update)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags(update);
                return View("~/Views/Dashboard/Serie/Edit.cshtml", update);
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

            return RedirectToAction("index", "seriemanagement");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator." + ex.Message);
            await PopulateViewBags(update);
            return View("~/Views/Dashboard/Serie/Edit.cshtml", update);
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
    [Route("/dashboard/serie/create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
        ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
        ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
        ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();

        return View("~/Views/Dashboard/Serie/Create.cshtml");
    }

    [HttpPost]
    [Route("/dashboard/serie/create")]

    public async Task<IActionResult> Create(CreateSerieModel serie)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Actors = await _unitOfWork.Repository<Actor>().GetAllAsync();
            ViewBag.Genres = await _unitOfWork.Repository<Genre>().GetAllAsync();
            ViewBag.Countries = await _unitOfWork.Repository<Country>().GetAllAsync();
            ViewBag.AgeCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync();
            return View("~/Views/Dashboard/Serie/Create.cshtml", serie);
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

        return RedirectToAction("index", "seriemanagement");
    }

}