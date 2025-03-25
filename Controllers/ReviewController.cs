using Microsoft.AspNetCore.Mvc;
using Netflex.Models.Review;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Netflex.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Action GET cho Film
        public async Task<IActionResult> GetFilmReview([FromRoute] Guid id)
        {
            var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createrId == null)
                return Unauthorized();

            if (id == Guid.Empty)
                return BadRequest("Invalid Film ID");

            var model = await GetFilmRating(id, createrId);
            return PartialView("_ReviewPartial", model);
        }

        // Action GET cho Series
        public async Task<IActionResult> GetSerieReview([FromRoute] Guid id)
        {
            var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (createrId == null)
                return Unauthorized();

            if (id == Guid.Empty)
                return BadRequest("Invalid Series ID");

            var model = await GetSerieRating(id, createrId);
            return PartialView("_ReviewPartial", model); // Có thể dùng partial view riêng nếu cần
        }

        // Action POST cho Film
        [HttpPost]
        public async Task<IActionResult> FilmRating([FromRoute] Guid id, [FromBody] ReviewEditModel model)
        {
            try
            {
                var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (createrId == null)
                    return Unauthorized();

                if (id == Guid.Empty)
                    return BadRequest("Invalid Film ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
                        SerieId = null // Đảm bảo SerieId là null khi rating cho Film
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
                return PartialView("_ReviewPartial", modelView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // Action POST cho Series
        [HttpPost]
        public async Task<IActionResult> SerieRating([FromRoute] Guid id, [FromBody] ReviewEditModel model)
        {
            try
            {
                var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (createrId == null)
                    return Unauthorized();

                if (id == Guid.Empty)
                    return BadRequest("Invalid Series ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
                        SerieId = id,  // Gán SerieId
                        FilmId = null  // Đảm bảo FilmId là null khi rating cho Series
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
                return PartialView("_ReviewPartial", modelView); // Có thể dùng partial view riêng nếu cần
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // Hàm hỗ trợ lấy rating cho Film
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
                SerieId = null // Đảm bảo SerieId là null
            };
        }

        // Hàm hỗ trợ lấy rating cho Series
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
                FilmId = null, // Đảm bảo FilmId là null
                SerieId = id
            };
        }
    }
}