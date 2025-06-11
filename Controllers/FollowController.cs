using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Netflex.Models.Follow;
using Netflex.Models.Film;
using Netflex.Models.Serie;
using Netflex.Models.User;
using X.PagedList.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : BaseController
    {
        private const int PAGE_SIZE = 5;
        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;

        public FollowController(IFollowRepository followRepository, IUnitOfWork unitOfWork, UserManager<User> userManager) : base(unitOfWork)
        {
            _followRepository = followRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowedItems(int? page)
        {
            int pageNumber = page ?? 1;
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var followedFilms = await _followRepository.GetByUserIdAsync(user.Id);

            var followedFilmViewModels = followedFilms
                .Where(f => f.FilmId != null)
                .Select(f => new FollowViewModel
                {
                    Id = f.Id,
                    FollowerId = f.FollowerId,
                    FilmId = f.FilmId,
                    FollowedAt = f.FollowedAt
                })
                .ToList();

            var followedSeries = followedFilms
                .Where(f => f.SerieId != null)
                .Select(f => new FollowViewModel
                {
                    Id = f.Id,
                    FollowerId = f.FollowerId,
                    SerieId = f.SerieId,
                    FollowedAt = f.FollowedAt
                })
                .ToList();

            foreach (var film in followedFilmViewModels)
            {
                var filmDetails = _unitOfWork.Repository<Film>().Entities
                    .Where(f => f.Id == film.FilmId)
                    .Select(f => new DetailFilmViewModel
                    {
                        Id = f.Id,
                        Title = f.Title,
                        About = f.About,
                        Poster = f.Poster,
                        Path = f.Path,
                        Trailer = f.Trailer,
                        ProductionYear = f.ProductionYear
                    }).FirstOrDefault();

                if (filmDetails != null)
                {
                    film.FollowedFilms = filmDetails;
                }
            }

            foreach (var serie in followedSeries)
            {
                var serieDetails = _unitOfWork.Repository<Serie>().Entities
                    .Where(s => s.Id == serie.SerieId)
                    .Select(s => new SerieViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Poster = s.Poster,
                        About = s.About,
                        AgeCategoryId = s.AgeCategoryId,
                        ProductionYear = s.ProductionYear
                    }).FirstOrDefault();

                if (serieDetails != null)
                {
                    serie.FollowedSeries = serieDetails;
                }
            }

            var combinedFollowedItems = followedFilmViewModels
                .Where(f => f.FollowedFilms != null)
                .Concat(followedSeries.Where(s => s.FollowedSeries != null))
                .ToPagedList(pageNumber, PAGE_SIZE);

            return Ok(new
            {
                items = combinedFollowedItems,
                pageNumber,
                pageSize = PAGE_SIZE,
                totalItems = combinedFollowedItems.TotalItemCount,
                totalPages = combinedFollowedItems.PageCount
            });
        }

        [HttpPost("film/{filmId}")]
        public async Task<IActionResult> AddFilm(Guid filmId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var filmExists = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, filmId);
            if (filmExists != null)
            {
                return BadRequest(new { message = "You are already following this film." });
            }

            var follow = new Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = user.Id,
                FilmId = filmId,
                FollowedAt = DateTime.UtcNow
            };

            await _followRepository.AddAsync(follow);
            await _followRepository.Save(CancellationToken.None);

            return Ok(new { message = "Film followed successfully", follow });
        }

        [HttpPost("serie/{serieId}")]
        public async Task<IActionResult> AddSerie(Guid serieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var serieExists = await _followRepository.GetByUserIdAndSerieIdAsync(user.Id, serieId);
            if (serieExists != null)
            {
                return BadRequest(new { message = "You are already following this serie." });
            }

            var follow = new Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = user.Id,
                SerieId = serieId,
                FollowedAt = DateTime.UtcNow
            };

            await _followRepository.AddAsync(follow);
            await _followRepository.Save(CancellationToken.None);

            return Ok(new { message = "Serie followed successfully", follow });
        }

        [HttpGet("check/{filmId}")]
        public async Task<IActionResult> CheckFollowStatus(Guid filmId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Ok(new { isFollowed = false });
            }

            var filmExists = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, filmId);

            return Ok(new { isFollowed = filmExists != null });
        }

        [HttpPost("toggle/film")]
        public async Task<IActionResult> ToggleFollowFilm([FromBody] Guid filmId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var existingFollow = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, filmId);
            if (existingFollow != null)
            {
                await _unitOfWork.Repository<Follow>().DeleteAsync(existingFollow);
                await _unitOfWork.Save(CancellationToken.None);
                return Ok(new { isFollowed = false, message = "Film unfollowed successfully" });
            }
            else
            {
                var follow = new Follow
                {
                    Id = Guid.NewGuid(),
                    FollowerId = user.Id,
                    FilmId = filmId,
                    FollowedAt = DateTime.UtcNow
                };
                await _followRepository.AddAsync(follow);
                await _followRepository.Save(CancellationToken.None);
                return Ok(new { isFollowed = true, message = "Film followed successfully" });
            }
        }

        [HttpPost("toggle/serie")]
        public async Task<IActionResult> ToggleFollowSerie([FromBody] Guid serieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var existingFollow = await _followRepository.GetByUserIdAndSerieIdAsync(user.Id, serieId);
            if (existingFollow != null)
            {
                await _unitOfWork.Repository<Follow>().DeleteAsync(existingFollow);
                await _unitOfWork.Save(CancellationToken.None);
                return Ok(new { isFollowed = false, message = "Serie unfollowed successfully" });
            }
            else
            {
                var follow = new Follow
                {
                    Id = Guid.NewGuid(),
                    FollowerId = user.Id,
                    SerieId = serieId,
                    FollowedAt = DateTime.UtcNow
                };
                await _followRepository.AddAsync(follow);
                await _followRepository.Save(CancellationToken.None);
                return Ok(new { isFollowed = true, message = "Serie followed successfully" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound(new { message = "Follow ID is required" });
            }

            var follow = _unitOfWork.Repository<Follow>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (follow == null)
            {
                return NotFound(new { message = "Follow not found" });
            }

            await _unitOfWork.Repository<Follow>().DeleteAsync(follow);
            await _unitOfWork.Save(CancellationToken.None);

            return Ok(new { message = "Follow deleted successfully" });
        }
    }
}