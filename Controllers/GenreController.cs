using Microsoft.AspNetCore.Mvc;
using Netflex.Entities;
using Netflex.Models.Genre;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PAGE_SIZE = 10;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetGenres(string? searchString, string? sortBy = "name", int pageNumber = 1)
        {
            var repository = _unitOfWork.Repository<Genre>();
            var query = repository.Entities;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchString.ToLower()));
            }

            query = sortBy switch
            {
                "name" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };

            var result = query.Select(x => new GenreViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToPagedList(pageNumber, PAGE_SIZE);

            return Ok(new
            {
                Genres = result,
                CurrentFilter = searchString,
                SortBy = sortBy
            });
        }

        [HttpGet("dropdown")]
        public IActionResult GetGenreDropdown()
        {
            try
            {
                var repository = _unitOfWork.Repository<Genre>();
                var genres = repository.Entities
                    .OrderBy(g => g.Name)
                    .Select(g => new GenreViewModel { Id = g.Id, Name = g.Name })
                    .ToList();

                return Ok(genres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenreEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new Genre { Id = Guid.NewGuid(), Name = model.Name };
            await _unitOfWork.Repository<Genre>().AddAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return CreatedAtAction(nameof(GetGenre), new { id = entity.Id }, entity);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenre(Guid id)
        {
            var genre = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found");
            }

            var model = new GenreViewModel { Id = genre.Id, Name = genre.Name };
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GenreEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound("Genre not found");
            }

            entity.Name = model.Name;
            await _unitOfWork.Repository<Genre>().UpdateAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _unitOfWork.Repository<Genre>().GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound("Genre not found");
            }

            await _unitOfWork.Repository<Genre>().DeleteAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }
    }
}