using Microsoft.AspNetCore.Mvc;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Database.Repositories.Implements;
using Netflex.Models.Country;
using X.PagedList;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private const int PageSize = 5; 

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            var countries = await _countryRepository.GetAllAsync();

            // Tìm kiếm nếu có từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                countries = countries.Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang
            var pageNumber = page ?? 1;
            var pagedCountries = countries.ToPagedList(pageNumber, PageSize);

            ViewBag.SearchString = searchString; 

            return View(pagedCountries);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateCountryViewModel country)
        {
            if (!ModelState.IsValid) return View(country);

            var newCountry = new Country
            {
                Id = Guid.NewGuid(),
                Name = country.Name
            };
            await _countryRepository.AddAsync(newCountry);
            await _countryRepository.SaveChangeAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null) return NotFound();
            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditCountryViewModel country)
        {
            if (!ModelState.IsValid) return View(country);

            var existingCountry = await _countryRepository.GetByIdAsync(id);
            if (existingCountry == null) return NotFound();

            existingCountry.Name = country.Name;

            await _countryRepository.UpdateAsync(existingCountry);
            await _countryRepository.SaveChangeAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null) return NotFound();
            return View(country);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null) return NotFound();

            await _countryRepository.DeleteAsync(country);
            await _countryRepository.SaveChangeAsync();

            return Json(new { success = true });
        }
    }
}
