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
        public async Task<IActionResult> GetReview(Guid id)
        {
            var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "00000000-0000-0000-0000-000000000000";

            if(id == Guid.Empty)
                return BadRequest("Invalid Film ID");

            var model = await GetRating(id, createrId);
            return PartialView("_ReviewPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> Rating(Guid id, [FromBody]ReviewEditModel model)
        {
            try
            {
                var createrId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "00000000-0000-0000-0000-000000000000";

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
                        FilmId = model.FilmId
                    };

                    await reviewRepo.AddAsync(newReview);
                }
                else
                {
                    existingReview.Rating = model.Rating;
                    await reviewRepo.UpdateAsync(existingReview);
                }

                await _unitOfWork.Save(CancellationToken.None);

                var modelView = await GetRating(id, createrId);
                return PartialView("_ReviewPartial", modelView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error" + ex.Message);
            }
            
        }

        private async Task<ReviewViewModel> GetRating(Guid id, string createrId)
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
                FilmId = id
            };
        }
    }
}
