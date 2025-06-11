using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Models.AgeCategory;

namespace Netflex.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgeCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PAGE_SIZE = 3;

        public AgeCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchString = null, string? sortBy = "name", int pageNumber = 1)
        {
            var repository = _unitOfWork.Repository<AgeCategory>();
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

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .Select(x => new AgeCategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = PAGE_SIZE,
                Items = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (category == null) return NotFound();

            var model = new AgeCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgeCategoryEditModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new AgeCategory
            {
                Id = Guid.NewGuid(),
                Name = model.Name
            };

            await _unitOfWork.Repository<AgeCategory>().AddAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] AgeCategoryEditModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            await _unitOfWork.Repository<AgeCategory>().UpdateAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _unitOfWork.Repository<AgeCategory>().GetByIdAsync(id);
            if (entity == null) return NotFound();

            var filmRepo = _unitOfWork.Repository<Film>();
            var serieRepo = _unitOfWork.Repository<Serie>();

            var filmsWithCategory = (await filmRepo.GetAllAsync())
                .Where(f => f.AgeCategoryId == id)
                .ToList();

            foreach (var film in filmsWithCategory)
            {
                film.AgeCategoryId = null;
                await filmRepo.UpdateAsync(film);
            }

            var seriesWithCategory = (await serieRepo.GetAllAsync())
                .Where(s => s.AgeCategoryId == id)
                .ToList();

            foreach (var serie in seriesWithCategory)
            {
                serie.AgeCategoryId = null;
                await serieRepo.UpdateAsync(serie);
            }

            await _unitOfWork.Repository<AgeCategory>().DeleteAsync(entity);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }
    }
}
