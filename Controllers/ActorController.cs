using Microsoft.AspNetCore.Mvc;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Models.Actor;

namespace Netflex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly IActorRepository _actorRepository;
        private readonly IStorageService _storage;
        private const int PageSize = 5;

        public ActorController(IActorRepository actorRepository, IStorageService storage)
        {
            _actorRepository = actorRepository;
            _storage = storage;
        }

        // GET: api/actor
        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchString = null, int page = 1)
        {
            var actors = await _actorRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                actors = actors
                    .Where(a => a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var pagedActors = actors
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Ok(new
            {
                CurrentPage = page,
                PageSize,
                Total = actors.Count,
                Data = pagedActors
            });
        }

        // GET: api/actor/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return Ok(actor);
        }

        // POST: api/actor
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? photoUrl = null;
            if (photoFile != null)
            {
                var photoUri = await _storage.UploadFileAsync("actor-photos", photoFile);
                photoUrl = photoUri?.ToString();
            }

            var newActor = new Actor
            {
                Id = Guid.NewGuid(),
                Name = actor.Name,
                About = actor.About,
                Photo = photoUrl
            };

            await _actorRepository.AddAsync(newActor);
            await _actorRepository.SaveChangeAsync();

            return CreatedAtAction(nameof(GetById), new { id = newActor.Id }, newActor);
        }

        // PUT: api/actor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EditActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingActor = await _actorRepository.GetByIdAsync(id);
            if (existingActor == null) return NotFound();

            existingActor.Name = actor.Name;
            existingActor.About = actor.About;

            if (photoFile != null)
            {
                var photoUri = await _storage.UploadFileAsync("actor-photos", photoFile);
                existingActor.Photo = photoUri?.ToString();
            }
            else
            {
                existingActor.Photo = actor.Photo;
            }

            await _actorRepository.UpdateAsync(existingActor);
            await _actorRepository.SaveChangeAsync();

            return NoContent();
        }

        // DELETE: api/actor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();

            await _actorRepository.DeleteAsync(actor);
            await _actorRepository.SaveChangeAsync();

            return NoContent();
        }
    }
}
