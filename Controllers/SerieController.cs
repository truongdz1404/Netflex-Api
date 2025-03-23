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
        private const int PAGE_SIZE = 10;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        private readonly IFollowRepository _followRepository;
        private readonly UserManager<User> _userManager;
        public SerieController(IFollowRepository followRepository, IUnitOfWork unitOfWork, ApplicationDbContext dbContext, UserManager<User> userManager) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
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
                .Include(s => s.SerieCountries)
                .Include(s => s.SerieGenres)
                .Include(s => s.SerieActors)
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
                CountryIds = _dbContext.SerieCountries.Where(x => x.SerieId == serie.Id).Select(x => x.CountryId).ToList(),
                GenreIds = _dbContext.SerieGenres.Where(x => x.SerieId == serie.Id).Select(x => x.GenreId).ToList(),
                ActorIds = _dbContext.SerieActors.Where(x => x.SerieId == serie.Id).Select(x => x.ActorId).ToList(),
                IsFollowed = isFollowed
            };
            ViewBag.Episodes = _unitOfWork.Repository<Episode>().Entities.Where(e => e.SerieId == id).ToList();
            return View(model);
        }
    }
}