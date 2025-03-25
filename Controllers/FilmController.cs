using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Netflex.Models.Film;
using Microsoft.AspNetCore.Identity;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class FilmController : BaseController
    {
        private const int PAGE_SIZE = 10;
        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;

        public FilmController(IFollowRepository followRepository, IUnitOfWork unitOfWork, UserManager<User> userManager) : base(unitOfWork)
        {
            _followRepository = followRepository;
            _userManager = userManager;
        }

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
                    ProductionYear = film.ProductionYear,
                    CreatedAt = film.CreatedAt

                }
            ).OrderBy(f => f.CreatedAt)
            .ToPagedList(pageNumber, PAGE_SIZE);

            return View(models);
        }
        public async Task<IActionResult> Detail(Guid? id)
        {
            if (id == null)
                return NotFound();
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isFollowed = false;

            if (user != null)
            {
                var existingFollow = await _followRepository.GetByUserIdAndFilmIdAsync(user.Id, film.Id);
                isFollowed = existingFollow != null;
            }
            var model = new DetailFilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                About = film.About,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear,
                IsFollowed = isFollowed
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchTitle"></param>
        /// <returns></returns>
        public IActionResult SearchByTitle(int? page, string? searchTitle)
        {
            int pageNumber = page ?? 1;
            var query = _unitOfWork.Repository<Film>().Entities;
            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(f => f.Title.ToLower().Contains(searchTitle));
            }

            var models = query.Select(
                film => new DetailFilmViewModel()
                {
                    Id = film.Id,
                    Title = film.Title,
                    About = film.About,
                    Poster = film.Poster,
                    Path = film.Path,
                    Trailer = film.Trailer,
                    ProductionYear = film.ProductionYear,
                }
            ).OrderBy(f => f.ProductionYear)
            .ToPagedList(pageNumber, PAGE_SIZE);

            return View(models);
        }
    }
}