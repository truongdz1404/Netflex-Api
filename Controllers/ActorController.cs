using Microsoft.AspNetCore.Mvc;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Models.Actor;
using X.PagedList;
using X.PagedList.Extensions;

namespace Netflex.Web.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorRepository _actorRepository;
        private readonly IStorageService _storage;
        private const int PageSize = 5;

        public ActorController(IActorRepository actorRepository, IStorageService storage)
        {
            _actorRepository = actorRepository;
            _storage = storage;
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var actors = await _actorRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                actors = actors
                    .Where(a => a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int pageNumber = page ?? 1;
            var pagedActors = actors.ToPagedList(pageNumber, PageSize);

            ViewBag.SearchString = searchString;
            return View(pagedActors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return View(actor);

            string? photoUrl = null;
            if (photoFile != null)
            {
                var photoUri = await _storage.UploadFileAsync("actor-photos", photoFile);
                photoUrl = photoUri?.ToString();
            }

            var newActor = new Actor()
            {
                Id = Guid.NewGuid(),
                Name = actor.Name,
                About = actor.About,
                Photo = photoUrl
            };
            await _actorRepository.AddAsync(newActor);
            await _actorRepository.SaveChangeAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(new EditActorViewModel
            {
                Id = actor.Id,
                Name = actor.Name,
                About = actor.About,
                Photo = actor.Photo
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return View(actor);

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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();

            await _actorRepository.DeleteAsync(actor);
            await _actorRepository.SaveChangeAsync();

            return Json(new { success = true });
        }
    }
}
