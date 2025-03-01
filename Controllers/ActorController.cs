using Microsoft.AspNetCore.Mvc;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Models.Actor;

namespace Netflex.Web.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorRepository _actorRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActorController(IActorRepository actorRepository, IWebHostEnvironment webHostEnvironment)
        {
            _actorRepository = actorRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Hiển thị danh sách diễn viên
        public async Task<IActionResult> Index()
        {
            var actors = await _actorRepository.GetAllAsync();
            return View(actors);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return View(actor);

            if (photoFile != null)
            {
                actor.Photo = await SavePhotoAsync(photoFile);
            }
            var newactor = new Actor()
            {
                Id = Guid.NewGuid(),
                Name = actor.Name,
                About = actor.About,
                Photo = actor.Photo,
            };
            await _actorRepository.AddAsync(newactor);
            await _actorRepository.SaveChangeAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Details(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        // Form chỉnh sửa diễn viên
        public async Task<IActionResult> Edit(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        // Xử lý cập nhật diễn viên
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditActorViewModel actor, IFormFile? photoFile)
        {
            if (!ModelState.IsValid) return View(actor);

            var existingActor = await _actorRepository.GetByIdAsync(id);
            if (existingActor == null) return NotFound();
            existingActor.Name = actor.Name;
            existingActor.About = actor.About;
            existingActor.Photo = actor.Photo;
            if (photoFile != null)
            {
                actor.Photo = await SavePhotoAsync(photoFile);
            }
            else
            {
                actor.Photo = existingActor.Photo;
            }
            
            await _actorRepository.UpdateAsync(existingActor);
            await _actorRepository.SaveChangeAsync();
            return RedirectToAction(nameof(Index));
        }

        // Form xác nhận xóa diễn viên
        public async Task<IActionResult> Delete(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        // Xử lý xóa diễn viên
        [HttpDelete]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null) return NotFound();

            await _actorRepository.DeleteAsync(actor);
            await _actorRepository.SaveChangeAsync();

            return Json(new { success = true });
        }

        private async Task<string> SavePhotoAsync(IFormFile photoFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}_{photoFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(fileStream);
            }

            return "/uploads/" + fileName;
        }
    }
}