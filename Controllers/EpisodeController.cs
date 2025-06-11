using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Episode;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly NotificationQueueService _notificationService;
        private const int PAGE_SIZE = 3;

        public EpisodeController(IStorageService storage, IUnitOfWork unitOfWork,
            ApplicationDbContext context, NotificationQueueService notificationService)
        {
            _storage = storage;
            _unitOfWork = unitOfWork;
            _context = context;
            _notificationService = notificationService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("serie/{serieId}")]
        public IActionResult GetEpisodes(string? searchTerm, string? sortOrder, int? page, Guid serieId)
        {
            int pageNumber = page ?? 1;
            var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(serieId));
            if (serie == null)
            {
                return NotFound("Serie not found");
            }

            var query = _unitOfWork.Repository<Episode>().Entities
                .Where(e => e.SerieId == serieId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            query = sortOrder switch
            {
                "title" => query.OrderBy(e => e.Title),
                "title_desc" => query.OrderByDescending(e => e.Title),
                "number" => query.OrderBy(e => e.Number),
                "number_desc" => query.OrderByDescending(e => e.Number),
                _ => query.OrderBy(e => e.Number)
            };

            var models = query.Select(episode => new EpisodeViewModel
            {
                Id = episode.Id,
                Title = episode.Title,
                Path = episode.Path,
                Number = episode.Number,
                About = episode.About,
                Serie = serie.Title
            }).ToPagedList(pageNumber, PAGE_SIZE);

            return Ok(new
            {
                Episodes = models,
                SerieTitle = serie.Title ?? "Không có serie",
                SerieId = serie.Id,
                SearchTerm = searchTerm,
                SortOrder = sortOrder
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEpisodeDetail(Guid id)
        {
            var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (episode == null)
            {
                return NotFound("Episode not found");
            }

            var serie = _unitOfWork.Repository<Serie>().Entities
                .Include(s => s.SerieGenres)
                .FirstOrDefault(m => m.Id == episode.SerieId);
            if (serie == null)
            {
                return NotFound("Serie not found");
            }

            var model = new DetailEpisodeViewModel
            {
                Title = episode.Title,
                About = episode.About,
                Number = episode.Number,
                File = episode.Path,
                Serie = serie.Title
            };

            var genreIds = _context.SerieGenres.Where(x => x.SerieId == episode.SerieId).Select(x => x.GenreId).ToList();
            var genres = _unitOfWork.Repository<Genre>().Entities.Where(g => genreIds.Contains(g.Id)).ToList();

            var actorIds = _context.SerieActors.Where(x => x.SerieId == episode.SerieId).Select(x => x.ActorId).ToList();
            var actors = _unitOfWork.Repository<Actor>().Entities.Where(g => actorIds.Contains(g.Id)).ToList();

            var episodes = _unitOfWork.Repository<Episode>().Entities.Where(e => e.SerieId == episode.SerieId).ToList();

            return Ok(new
            {
                Episode = model,
                Genres = genres,
                Actors = actors,
                Episodes = episodes,
                SerieTitle = serie.Title ?? "Không có serie",
                SerieId = serie.Id
            });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (episode == null)
            {
                return NotFound("Episode not found");
            }

            await _unitOfWork.Repository<Episode>().DeleteAsync(episode);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("edit/{id}")]
        public IActionResult GetEpisodeForEdit(Guid id)
        {
            var episode = _unitOfWork.Repository<Episode>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (episode == null)
            {
                return NotFound("Episode not found");
            }

            var model = new EditEpisodeViewModel
            {
                Id = episode.Id,
                Title = episode.Title,
                About = episode.About,
                FileUrl = episode.Path,
                Number = episode.Number,
                SerieId = episode.SerieId
            };

            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EditEpisodeViewModel update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var episode = await _unitOfWork.Repository<Episode>().GetByIdAsync(id);
            if (episode == null)
            {
                return NotFound("Episode not found");
            }

            var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id == episode.SerieId);
            if (serie == null)
            {
                return NotFound("Serie not found");
            }

            var episodeUri = update.File != null ? await _storage.UploadFileAsync("episode", update.File) : null;

            episode.Title = update.Title;
            episode.About = update.About;
            episode.Number = update.Number;
            episode.Path = episodeUri?.ToString() ?? update.FileUrl;

            await _unitOfWork.Save(CancellationToken.None);

            return Ok(new
            {
                Episode = episode,
                SerieTitle = serie.Title ?? "Không có serie",
                SerieId = serie.Id
            });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("serie/{serieId}")]
        public async Task<IActionResult> Create(Guid serieId, [FromForm] CreateEpisodeModel episode)
        {
            var serie = _unitOfWork.Repository<Serie>().Entities.FirstOrDefault(m => m.Id.Equals(serieId));
            if (serie == null)
            {
                return NotFound("Serie not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var episodeUri = episode.File != null ? await _storage.UploadFileAsync("episode", episode.File) : null;

            var newEpisode = new Episode
            {
                Id = Guid.NewGuid(),
                Title = episode.Title,
                About = episode.About,
                Path = episodeUri?.ToString() ?? string.Empty,
                SerieId = serieId,
                Number = episode.Number
            };

            await _unitOfWork.Repository<Episode>().AddAsync(newEpisode);
            await _unitOfWork.Save(CancellationToken.None);

            await NotifyNewEpisode(newEpisode, serie);

            return CreatedAtAction(nameof(GetEpisodeDetail), new { id = newEpisode.Id }, new
            {
                Episode = newEpisode,
                SerieTitle = serie.Title ?? "Không có serie",
                SerieId = serie.Id
            });
        }

        private async Task NotifyNewEpisode(Episode newEpisode, Serie serie)
        {
            var sendTo = _context.Follows
                .Where(f => f.SerieId == serie.Id)
                .Select(f => f.FollowerId)
                .ToList();

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(new
                {
                    Message = "A new episode has been added to the serie: " + serie.Title,
                    Link = $"/episode/detail?id={newEpisode.Id}"
                }),
                CreatedAt = DateTime.UtcNow,
                Status = "System"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            foreach (var id in sendTo)
            {
                _context.UserNotifications.Add(new UserNotification
                {
                    UserId = id,
                    NotificationId = notification.Id,
                    HaveRead = false
                });
            }

            await _context.SaveChangesAsync();
            await _notificationService.PushAsync(new Message(sendTo, notification.Content));
        }
    }
}