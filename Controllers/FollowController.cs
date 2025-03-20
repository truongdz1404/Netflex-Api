using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Netflex.Models.Follow;
using Netflex.Models.Film;
using Netflex.Models.Serie;
using Netflex.Models.User;
using X.PagedList.Extensions;
namespace Netflex.Controllers
{
    public class FollowController : BaseController
    {
        private const int PAGE_SIZE = 10;
        private readonly IFollowRepository _followRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public FollowController(IFollowRepository followRepository, IUnitOfWork unitOfWork, UserManager<User> userManager) : base(unitOfWork)
        {
            _followRepository = followRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // public async Task<IActionResult> Index(int? page)
        // {
        //     int pageNumber = page ?? 1;
        //     var user = await _userManager.GetUserAsync(User);

        //     if (user == null)
        //     {
        //         return Redirect("/Identity/Account/Login");
        //     }

        //     var followedFilms = await _followRepository.GetByUserIdAsync(user.Id);

        //     var followedFilmViewModels = followedFilms
        //         .Where(f => f.FilmId != null)
        //         .Select(f => new FollowViewModel
        //         {
        //             FollowerId = f.FollowerId,
        //             FilmId = f.FilmId,
        //             FollowedAt = f.FollowedAt
        //         })
        //         .ToList();

        //     var followedSeries = followedFilms
        //         .Where(f => f.SerieId != null)
        //         .Select(f => new FollowViewModel
        //         {
        //             FollowerId = f.FollowerId,
        //             SerieId = f.SerieId,
        //             FollowedAt = f.FollowedAt
        //         })
        //         .ToList();

        //     foreach (var film in followedFilmViewModels)
        //     {
        //         var models = _unitOfWork.Repository<Film>().Entities.Select(
        //         film => new FilmViewModel()
        //         {
        //             Id = film.Id,
        //             Title = film.Title,
        //             Poster = film.Poster,
        //             Path = film.Path,
        //             Trailer = film.Trailer,
        //             ProductionYear = film.ProductionYear,
        //             CreatedAt = film.CreatedAt

        //         }
        //     ).OrderBy(f => f.CreatedAt)
        //     .ToPagedList(pageNumber, PAGE_SIZE);
        //     }

        //     foreach (var serie in followedSeries)
        //     {
        //         var models = _unitOfWork.Repository<Serie>().Entities.Select(
        //         serie => new SerieViewModel()
        //         {
        //             Id = serie.Id,
        //             Title = serie.Title,
        //             Poster = serie.Poster,
        //             About = serie.About,
        //             AgeCategoryId = serie.AgeCategoryId,
        //             ProductionYear = serie.ProductionYear
        //         }
        //     ).ToPagedList(pageNumber, PAGE_SIZE);

        //     }

        //     var combinedFollowedItems = followedFilmViewModels.Concat(followedSeries).ToList();

        //     return View(combinedFollowedItems.ToPagedList(pageNumber, PAGE_SIZE));
        // }
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var followedFilms = await _followRepository.GetByUserIdAsync(user.Id);

            var followedFilmViewModels = followedFilms
                .Where(f => f.FilmId != null)
                .Select(f => new FollowViewModel
                {
                    FollowerId = f.FollowerId,
                    FilmId = f.FilmId,
                    FollowedAt = f.FollowedAt
                })
                .ToList();

            var followedSeries = followedFilms
                .Where(f => f.SerieId != null)
                .Select(f => new FollowViewModel
                {
                    FollowerId = f.FollowerId,
                    SerieId = f.SerieId,
                    FollowedAt = f.FollowedAt
                })
                .ToList();

            foreach (var film in followedFilmViewModels)
            {
                var filmDetails = _unitOfWork.Repository<Film>().Entities
                    .Where(f => f.Id == film.FilmId)
                    .Select(f => new FilmViewModel()
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Poster = f.Poster,
                        Path = f.Path,
                        Trailer = f.Trailer,
                        ProductionYear = f.ProductionYear,
                        CreatedAt = f.CreatedAt
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
                    .Select(s => new SerieViewModel()
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
                .ToList();

            return View(combinedFollowedItems.ToPagedList(pageNumber, PAGE_SIZE));
        }


        public async Task<IActionResult> AddFilm(Guid filmId, Guid serieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var filmExists = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, filmId);
            if (filmExists != null)
            {
                return BadRequest("You are already following this film.");
            }

            var follow = new Follow
            {
                FollowerId = user.Id,
                FilmId = filmId,
                SerieId = serieId,
                FollowedAt = DateTime.UtcNow
            };

            await _followRepository.AddAsync(follow);
            await _followRepository.Save(CancellationToken.None);

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> AddSerie(Guid serieId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var serieExists = await _followRepository.GetByUserIdAndSerieIdAsync(user.Id, serieId);
            if (serieExists != null)
            {
                return BadRequest("You are already following this series.");
            }

            var follow = new Follow
            {
                FollowerId = user.Id,
                FilmId = serieId,
                SerieId = serieId,
                FollowedAt = DateTime.UtcNow
            };

            await _followRepository.AddAsync(follow);
            return RedirectToAction(nameof(Index));
        }


        // DELETE: FollowController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FollowController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

