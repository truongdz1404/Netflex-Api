using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Review;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("film/{id}")]
        public async Task<IActionResult> GetFilmReview(Guid id)
        {
            var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createrId == null)
                return Unauthorized(new { message = "User not authenticated" });

            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid Film ID" });

            var model = await GetFilmRating(id, createrId);
            return Ok(model);
        }

        [HttpGet("serie/{id}")]
        public async Task<IActionResult> GetSerieReview(Guid id)
        {
            var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createrId == null)
                return Unauthorized(new { message = "User not authenticated" });

            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid Series ID" });

            var model = await GetSerieRating(id, createrId);
            return Ok(model);
        }

        [HttpPost("film/{id}")]
        public async Task<IActionResult> FilmRating(Guid id, [FromBody] ReviewEditModel model)
        {
            try
            {
                var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (createrId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Film ID" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid request data", errors = ModelState });

                var reviewRepo = _unitOfWork.Repository<Review>();
                var existingReview = await reviewRepo.Entities
                    .FirstOrDefaultAsync(x => x.FilmId == id && x.CreaterId == createrId);

                if (existingReview == null)
                {
                    var newReview = new Review
                    {
                        Id = Guid.NewGuid(),
                        Rating = model.Rating,
                        CreaterId = createrId,
                        FilmId = id,
                        SerieId = null
                    };
                    await reviewRepo.AddAsync(newReview);
                }
                else
                {
                    existingReview.Rating = model.Rating;
                    await reviewRepo.UpdateAsync(existingReview);
                }
                await _unitOfWork.Save(CancellationToken.None);

                var modelView = await GetFilmRating(id, createrId);
                return Ok(new { message = "Film review saved successfully", review = modelView });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the film review", error = ex.Message });
            }
        }

        [HttpPost("serie/{id}")]
        public async Task<IActionResult> SerieRating(Guid id, [FromBody] ReviewEditModel model)
        {
            try
            {
                var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (createrId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Series ID" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid request data", errors = ModelState });

                var reviewRepo = _unitOfWork.Repository<Review>();
                var existingReview = await reviewRepo.Entities
                    .FirstOrDefaultAsync(x => x.SerieId == id && x.CreaterId == createrId);

                if (existingReview == null)
                {
                    var newReview = new Review
                    {
                        Id = Guid.NewGuid(),
                        Rating = model.Rating,
                        CreaterId = createrId,
                        SerieId = id,
                        FilmId = null
                    };
                    await reviewRepo.AddAsync(newReview);
                }
                else
                {
                    existingReview.Rating = model.Rating;
                    await reviewRepo.UpdateAsync(existingReview);
                }
                await _unitOfWork.Save(CancellationToken.None);

                var modelView = await GetSerieRating(id, createrId);
                return Ok(new { message = "Series review saved successfully", review = modelView });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the series review", error = ex.Message });
            }
        }

        private async Task<ReviewViewModel> GetFilmRating(Guid id, string createrId)
        {
            var reviews = await _unitOfWork.Repository<Review>().Entities
                .Where(f => f.FilmId == id)
                .ToListAsync();

            var userReview = reviews.FirstOrDefault(r => r.CreaterId == createrId);

            return new ReviewViewModel
            {
                Rating = userReview?.Rating ?? 0,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 1,
                TotalReviews = reviews.Count,
                FilmId = id,
                SerieId = null
            };
        }

        private async Task<ReviewViewModel> GetSerieRating(Guid id, string createrId)
        {
            var reviews = await _unitOfWork.Repository<Review>().Entities
                .Where(s => s.SerieId == id)
                .ToListAsync();

            var userReview = reviews.FirstOrDefault(r => r.CreaterId == createrId);

            return new ReviewViewModel
            {
                Rating = userReview?.Rating ?? 0,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 1,
                TotalReviews = reviews.Count,
                FilmId = null,
                SerieId = id
            };
        }
    }
}