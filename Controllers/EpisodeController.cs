using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Episode;
using X.PagedList.Extensions;
namespace Netflex.Controllers;

public class EpisodeController(IStorageService storage, IUnitOfWork unitOfWork, ApplicationDbContext context)
    : BaseController(unitOfWork)
{
    private readonly IStorageService _storage = storage;
    private readonly ApplicationDbContext _context = context;
    private const int PAGE_SIZE = 3;
    [Authorize(Roles = "admin")]
    public IActionResult Index(int? page, Guid serieId)
    {
        int pageNumber = page ?? 1;
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(serieId)) ?? throw new NullReferenceException("Serie not found");
        var models = _unitOfWork.Repository<Episode>().Entities.Where(e => e.SerieId == serieId).Select(
            episode => new EpisodeViewModel()
            {
                Id = episode.Id,
                Title = episode.Title,
                Path = episode.Path,
                Number = episode.Number,
                About = episode.About,
                Serie = serie.Title
            }
        ).OrderBy(e => e.Number).ToPagedList(pageNumber, PAGE_SIZE);
        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;
        return View(models);
    }

    public IActionResult Detail(Guid? id)
    {
        if (id == null)
            return NotFound();
        var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (episode == null)
            return NotFound();
        var serie = _unitOfWork.Repository<Serie>().Entities.Include(s => s.SerieGenres)
        .FirstOrDefault(m => m.Id == (episode.SerieId)) ?? throw new NullReferenceException("Serie not found");

        var model = new DetailEpisodeViewModel
        {
            Title = episode.Title,
            About = episode.About,
            Number = episode.Number,
            File = episode.Path,
            Serie = serie.Title
        };
        var genreIds = _context.SerieGenres.Where(x => x.SerieId == serie.Id).Select(x => x.GenreId).ToList();
        ViewBag.Genres = _unitOfWork.Repository<Genre>().Entities.Where(g => genreIds.Contains(g.Id)).ToList();
        Console.WriteLine($"SerieGenres Count: {serie.SerieGenres?.Count()}");

        ViewBag.Episodes = _unitOfWork.Repository<Episode>().Entities.Where(e => e.SerieId == episode.SerieId).ToList();
        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;
        return View(model);
    }
    [Authorize(Roles = "admin")]
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
            return NotFound();
        var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (episode == null)
            return NotFound();
        await _unitOfWork.Repository<Episode>().DeleteAsync(episode);
        await _unitOfWork.Save(CancellationToken.None);
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(episode.SerieId)) ?? throw new NullReferenceException("Serie not found");
        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;
        return RedirectToAction("Index", "Episode", new { serieId = serie.Id });

    }
    [Authorize(Roles = "admin")]
    public IActionResult Edit(Guid? id)
    {
        if (id == null)
            return NotFound();
        var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
        if (episode == null)
            return NotFound();
        var model = new EditEpisodeViewModel
        {
            Title = episode.Title,
            About = episode.About,
            FileUrl = episode.Path,
            Number = episode.Number,
            SerieId = episode.SerieId,
        };
        return View(model);
    }
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(EditEpisodeViewModel update)
    {
        if (!ModelState.IsValid)
        {
            return View(update);
        }
        var episode = await _unitOfWork.Repository<Episode>().GetByIdAsync(update.Id);
        if (episode == null)
            return NotFound();
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id == episode.SerieId) ?? throw new NullReferenceException("Serie not found");
        var episodeUri = update.File != null ? await _storage.UploadFileAsync("episode", update.File) : null;
        episode.Title = update.Title;
        episode.About = update.About;
        episode.Number = update.Number;
        episode.Path = episodeUri?.ToString() ?? update.FileUrl;
        await _context.SaveChangesAsync();
        await _unitOfWork.Save(CancellationToken.None);
        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;
        return RedirectToAction("Index", "Episode", new { serieId = serie.Id });

    }

    public IActionResult Create(Guid serieId, string serieTitle)
    {
        ViewData["SerieTitle"] = serieTitle;
        ViewData["SerieId"] = serieId;
        return View();
    }
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEpisodeModel episode)
    {
        if (!ModelState.IsValid)
        {
            return View(episode);
        }

        var episodeUri = episode.File != null ? await _storage.UploadFileAsync("episode", episode.File) : null;

        var newEpisode = new Episode()
        {
            Id = Guid.NewGuid(),
            Title = episode.Title,
            About = episode.About,
            Path = episodeUri?.ToString() ?? string.Empty,
            SerieId = episode.SerieId,
            Number = episode.Number

        };
        await _unitOfWork.Repository<Episode>().AddAsync(newEpisode);
        await _unitOfWork.Save(CancellationToken.None);
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(episode.SerieId)) ?? throw new NullReferenceException("Serie not found");

        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;
        return RedirectToAction("Index", "Episode", new { serieId = serie.Id });
    }


    private void NotifyNewEpisode(Notification notification, Guid serieId)
    {

        // var sendTo = new string[] { userId };

    }

}