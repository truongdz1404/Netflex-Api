using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Episode;
using X.PagedList.Extensions;
namespace Netflex.Controllers;

public class EpisodeController : BaseController
{
    private readonly IStorageService _storage;
    private readonly ApplicationDbContext _context;
    private readonly NotificationQueueService _notificationService;

    private const int PAGE_SIZE = 3;

    public EpisodeController(IStorageService storage, IUnitOfWork unitOfWork,
        ApplicationDbContext context, NotificationQueueService notificationService) : base(unitOfWork)
    {
        _storage = storage;
        _context = context;
        _notificationService = notificationService;
    }

    [Authorize(Roles = "admin")]
    [Route("/dashboard/episode/index/{serieId}")]
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
        return View("~/Views/Dashboard/Episode/Index.cshtml", models);
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
    [Route("/dashboard/episode/delete/{id}")]
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
    [Route("/dashboard/episode/edit/{id}")]
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
        return View("~/Views/Dashboard/Episode/Edit.cshtml", model);
    }
    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("/dashboard/episode/edit/{id}")]
    public async Task<IActionResult> Edit(EditEpisodeViewModel update)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Dashboard/Episode/Edit.cshtml", update);
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

    [Route("/dashboard/episode/create/{serieId}/{serieTitle}")]
    [Authorize(Roles = "admin")]
    public IActionResult Create(Guid serieId, string serieTitle)
    {
        ViewData["SerieTitle"] = serieTitle;
        ViewData["SerieId"] = serieId;
        return View();
    }
    [Authorize(Roles = "admin")]
    [Route("/dashboard/episode/create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateEpisodeModel episode)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Dashboard/Episode/Create.cshtml", episode);
        }

        var episodeUri = episode.File != null ? await _storage.UploadFileAsync("episode", episode.File) : null;

        var newEpisode = new Episode()
        {
            Id = Guid.NewGuid(),
            Title = episode.Title,
            About = episode.About,
            Path = episodeUri?.ToString() ?? string.Empty,
            SerieId = episode.SerieId,
            Number = episode.Number,

        };
        await _unitOfWork.Repository<Episode>().AddAsync(newEpisode);
        await _unitOfWork.Save(CancellationToken.None);
        var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(episode.SerieId)) ?? throw new NullReferenceException("Serie not found");

        ViewData["SerieTitle"] = serie.Title ?? "Không có serie";
        ViewData["SerieId"] = serie.Id;

        await NotifyNewEpisode(newEpisode, serie);

        return RedirectToAction("Index", "Episode", new { serieId = serie.Id });
    }


    private async Task NotifyNewEpisode(Episode newEpisode, Serie serie)
    {
        var sendTo = _context.Follows.Where(f => f.SerieId == serie.Id)
            .Select(f => f.FollowerId).ToList();

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Content = JsonSerializer.Serialize(new { Message = "A new episode has been added to the serie: " + serie.Title, Link = $"/episode/detail?id={newEpisode.Id}" }),
            CreatedAt = DateTime.Now,
            Status = "System"
        };

        _context.Notifications.Add(notification);
        _context.SaveChanges();

        foreach (var id in sendTo)
        {
            _context.UserNotifications.Add(new UserNotification { UserId = id, NotificationId = notification.Id, HaveRead = false });
        }

        _context.SaveChanges();
        await _notificationService.PushAsync(new Message(sendTo, notification.Content));
    }

}