﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Netflex.Database;
using Microsoft.EntityFrameworkCore;



namespace Netflex.Controllers
{
    [Route("api/favorite")]
    [ApiController]
    public class FavoriteFilmsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoriteFilmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FavoriteFilms/user/abc123
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<FavoriteResultDto>> GetFavoriteFilmsByUser(string userId)
        {
            if (userId == null)
            {
                return NotFound("Thiếu thông tin UserId");
            }
            var favorites = await _context.FavoriteFilms
                .Include(f => f.Film)
                .Include(f => f.Serie)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            if (favorites == null || !favorites.Any())
            {
                return NotFound("Không có phim yêu thích nào.");
            }

            var result = new FavoriteResultDto
            {
                UserId = userId,
                FavoriteFilms = favorites
                    .Where(f => f.Film != null)
                    .Select(f => new FavoriteFilmDto
                    {
                        FilmId = f.FilmId!.Value,
                        Title = f.Film!.Title,
                        Poster = f.Film.Poster
                    })
                    .ToList(),
                FavoriteSeries = favorites
                    .Where(f => f.Serie != null)
                    .Select(f => new FavoriteSeriesDto
                    {
                        SeriesId = f.SeriesId!.Value,
                        Title = f.Serie!.Title,
                        Poster = f.Serie.Poster
                    })
                    .ToList()
            };

            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] AddFavoriteFilmDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId) || (dto.FilmId == null && dto.SeriesId == null))
            {
                return BadRequest(new { message = "Thiếu thông tin UserId hoặc Film/Serie." });
            }

            var exists = await _context.FavoriteFilms.AnyAsync(f =>
                f.UserId == dto.UserId &&
                ((dto.FilmId != null && f.FilmId == dto.FilmId) ||
                 (dto.SeriesId != null && f.SeriesId == dto.SeriesId)));

            if (exists)
            {
                return Conflict(new { message = "Phim hoặc series đã có trong danh sách yêu thích." });
            }

            if (dto.FilmId.HasValue)
            {
                var filmExists = await _context.Films.AnyAsync(f => f.Id == dto.FilmId.Value);
                if (!filmExists)
                {
                    return NotFound(new { message = "Film không tồn tại." });
                }
            }

            if (dto.SeriesId.HasValue)
            {
                var seriesExists = await _context.Series.AnyAsync(s => s.Id == dto.SeriesId.Value);
                if (!seriesExists)
                {
                    return NotFound(new { message = "Series không tồn tại." });
                }
            }

            var favorite = new FavoriteFilms
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                FilmId = dto.FilmId,
                SeriesId = dto.SeriesId,
            };

            _context.FavoriteFilms.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm vào danh sách yêu thích." });
        }





        // DELETE: api/FavoriteFilms/film?userId=abc123&filmId=xxx-guid
        [HttpDelete("remove")]
        public async Task<IActionResult> Remove(string userId, Guid? filmId, Guid? seriesId)
        {
            FavoriteFilms? favorite = null;

            if (filmId.HasValue)
            {
                favorite = await _context.FavoriteFilms.FirstOrDefaultAsync(f =>
                    f.UserId == userId && f.FilmId == filmId);
            }
            else if (seriesId.HasValue)
            {
                favorite = await _context.FavoriteFilms.FirstOrDefaultAsync(f =>
                    f.UserId == userId && f.SeriesId == seriesId);
            }

            if (favorite == null)
            {
                return NotFound(new { message = "Không tìm thấy mục yêu thích cần xoá." });
            }

            _context.FavoriteFilms.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xoá khỏi yêu thích." });
        }





        [HttpGet("is-favorite")]
        public async Task<IActionResult> IsFavorite([FromQuery] string userId, [FromQuery] Guid? filmId, [FromQuery] Guid? seriesId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Thiếu userId");

            if (!filmId.HasValue && !seriesId.HasValue)
                return BadRequest("Cần cung cấp filmId hoặc seriesId");

            if (filmId.HasValue && seriesId.HasValue)
                return BadRequest("Chỉ nên truyền filmId hoặc seriesId, không phải cả hai.");

            bool exists = false;

            if (filmId.HasValue)
            {
                exists = await _context.FavoriteFilms
                    .AnyAsync(f => f.UserId == userId && f.FilmId == filmId);
            }
            else if (seriesId.HasValue)
            {
                exists = await _context.FavoriteFilms
                    .AnyAsync(f => f.UserId == userId && f.SeriesId == seriesId);
            }

            return Ok(exists);
        }




        public class AddFavoriteFilmDto
        {
            public string UserId { get; set; } = null!;
            public Guid? FilmId { get; set; }
            public Guid? SeriesId { get; set; }
        }

        public class FavoriteResultDto
        {
            public string UserId { get; set; } = null!;
            public List<FavoriteFilmDto> FavoriteFilms { get; set; } = new();
            public List<FavoriteSeriesDto> FavoriteSeries { get; set; } = new();
        }

        public class FavoriteFilmDto
        {
            public Guid FilmId { get; set; }
            public string Title { get; set; } = null!;
            public string? Poster { get; set; }

        }

        public class FavoriteSeriesDto
        {
            public Guid SeriesId { get; set; }
            public string Title { get; set; } = null!;
            public string? Poster { get; set; }
        }



    }
}
