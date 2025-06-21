using Microsoft.AspNetCore.Mvc;
using Netflex.Database.Repositories.Abstractions;
using Netflex.Models.Country;
using X.PagedList;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private const int PageSize = 12;

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchString, int? page)
        {
            var countries = await _countryRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                countries = countries.Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pageNumber = page ?? 1;
            var pagedCountries = countries.ToPagedList(pageNumber, PageSize);

            return Ok(pagedCountries);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCountryViewModel country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCountry = new Country
            {
                Id = Guid.NewGuid(),
                Name = country.Name
            };

            await _countryRepository.AddAsync(newCountry);
            await _countryRepository.SaveChangeAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCountry.Id }, newCountry);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EditCountryViewModel country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCountry = await _countryRepository.GetByIdAsync(id);
            if (existingCountry == null)
            {
                return NotFound();
            }

            existingCountry.Name = country.Name;

            await _countryRepository.UpdateAsync(existingCountry);
            await _countryRepository.SaveChangeAsync();

            return Ok(existingCountry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            await _countryRepository.DeleteAsync(country);
            await _countryRepository.SaveChangeAsync();

            return NoContent();
        }
    }
}