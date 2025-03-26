using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netflex.Database;
using Netflex.Models.Serie;
using Microsoft.AspNetCore.Identity;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class SerieController : BaseController
    {
        private const int PAGE_SIZE = 12;
        private readonly ApplicationDbContext _context;

        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;
        public SerieController(IFollowRepository followRepository, IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<User> userManager) : base(unitOfWork)
        {
            _context = context;
            _followRepository = followRepository;
            _userManager = userManager;
        }

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

        public async Task<IActionResult> Detail(Guid? id)
        {
            if (id == null)
                return NotFound();
            var serie = _unitOfWork.Repository<Serie>().Entities
                .FirstOrDefault(m => m.Id.Equals(id));
            if (serie == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isFollowed = false;

            if (user != null)
            {
                var existingFollow = await _followRepository.GetByUserIdAndSerieIdAsync(user.Id, serie.Id);
                isFollowed = existingFollow != null;
            }
            var model = new DetailSerieViewModel
            {
                Id = serie.Id,
                Title = serie.Title,
                About = serie.About,
                Poster = serie.Poster,
                AgeCategoryId = serie.AgeCategoryId,
                ProductionYear = serie.ProductionYear,
                CountryIds = _context.SerieCountries.Where(x => x.SerieId == serie.Id).Select(x => x.CountryId).ToList(),
                GenreIds = _context.SerieGenres.Where(x => x.SerieId == serie.Id).Select(x => x.GenreId).ToList(),
                ActorIds = _context.SerieActors.Where(x => x.SerieId == serie.Id).Select(x => x.ActorId).ToList(),
                IsFollowed = isFollowed

            };
            ViewBag.Episodes = _unitOfWork.Repository<Episode>().Entities.Where(e => e.SerieId == id).ToList();

            var actorIds = _context.SerieActors
                           .Where(fa => fa.SerieId == id)
                           .Select(fa => fa.ActorId)
                           .ToList();

            ViewBag.Actors = _unitOfWork.Repository<Actor>()
                .Entities
                .Where(a => actorIds.Contains(a.Id))
                .ToList();

            var genreIds = _context.SerieGenres
                .Where(fg => fg.SerieId == id)
                .Select(fg => fg.GenreId)
                .ToList();

            ViewBag.Genres = _unitOfWork.Repository<Genre>()
                .Entities
                .Where(g => genreIds.Contains(g.Id))
                .ToList();

            var relatedSerieIds = _context.SerieGenres
            .Where(fg => genreIds.Contains(fg.GenreId) && fg.SerieId != id)
            .Select(fg => fg.SerieId)
            .Distinct()
            .Take(50)
            .ToList();

            var relatedSeries = _unitOfWork.Repository<Serie>()
                .Entities
                .Where(f => relatedSerieIds.Contains(f.Id))
                .OrderBy(f => f.Title)
                .Take(10)
                .ToList();

            ViewBag.RelatedSeries = relatedSeries;

            return View(model);
        }
    }
}