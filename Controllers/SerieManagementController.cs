using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Database;
using Netflex.Models.Serie;
using OfficeOpenXml;
using X.PagedList.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SerieManagementController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private const int PAGE_SIZE = 6;

        public SerieManagementController(IStorageService storage, IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            _storage = storage;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetSeries(string? searchTerm, int? productionYear, string? sortOrder, int? page)
        {
            try
            {
                int pageNumber = page ?? 1;
                var query = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => s.Title.ToLower().Contains(searchTerm.ToLower()));
                }

                if (productionYear.HasValue)
                {
                    query = query.Where(s => s.ProductionYear == productionYear.Value);
                }

                query = sortOrder switch
                {
                    "title" => query.OrderBy(s => s.Title),
                    "title_desc" => query.OrderByDescending(s => s.Title),
                    "production_year" => query.OrderBy(s => s.ProductionYear),
                    "production_year_desc" => query.OrderByDescending(s => s.ProductionYear),
                    _ => query.OrderBy(s => s.Title)
                };

                var series = await query
       .ToListAsync();

                var models = series
                    .Select(serie => new SerieViewModel
                    {
                        Id = serie.Id,
                        Title = serie.Title,
                        Poster = serie.Poster,
                        About = serie.About,
                        AgeCategoryId = serie.AgeCategoryId,
                        ProductionYear = serie.ProductionYear
                    })
                    .ToPagedList(pageNumber, PAGE_SIZE);

                return Ok(new
                {
                    items = models,
                    pageNumber,
                    pageSize = PAGE_SIZE,
                    totalItems = models.TotalItemCount,
                    totalPages = models.PageCount,
                    searchTerm,
                    productionYear,
                    sortOrder
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching series", error = ex.Message });
            }
        }

        [HttpGet("export")]
        public IActionResult ExportToExcel(string? searchTerm, int? productionYear, string? sortOrder)
        {
            try
            {
                var query = _unitOfWork.Repository<Serie>().Entities.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(f => f.Title.Contains(searchTerm));
                }

                if (productionYear.HasValue)
                {
                    query = query.Where(f => f.ProductionYear == productionYear.Value);
                }

                query = sortOrder switch
                {
                    "title" => query.OrderBy(f => f.Title),
                    "title_desc" => query.OrderByDescending(f => f.Title),
                    "production_year" => query.OrderBy(f => f.ProductionYear),
                    "production_year_desc" => query.OrderByDescending(f => f.ProductionYear),
                    _ => query.OrderBy(f => f.Title)
                };

                var series = query.Select(f => new SerieViewModel
                {
                    Id = f.Id,
                    Title = f.Title,
                    Poster = f.Poster,
                    ProductionYear = f.ProductionYear
                }).ToList();

                if (!series.Any())
                {
                    return NotFound(new { message = "No series found for export" });
                }

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Series");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "Production Year";
                worksheet.Cells[1, 4].Value = "Poster Link";

                for (int i = 0; i < series.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = series[i].Id;
                    worksheet.Cells[i + 2, 2].Value = series[i].Title;
                    worksheet.Cells[i + 2, 3].Value = series[i].ProductionYear;
                    worksheet.Cells[i + 2, 4].Value = series[i].Poster;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Series-{Guid.NewGuid()}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while exporting series", error = ex.Message });
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetSerieDetail(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Series ID" });

                var serie = await _unitOfWork.Repository<Serie>().Entities
                    .Include(s => s.SerieCountries)
                    .Include(s => s.SerieGenres)
                    .Include(s => s.SerieActors)
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

                if (serie == null)
                    return NotFound(new { message = "Series not found" });

                var model = new DetailSerieViewModel
                {
                    Id = serie.Id,
                    Title = serie.Title,
                    About = serie.About,
                    Poster = serie.Poster,
                    AgeCategoryId = serie.AgeCategoryId,
                    ProductionYear = serie.ProductionYear,
                    CountryIds = serie.SerieCountries.Select(x => x.CountryId).ToList(),
                    GenreIds = serie.SerieGenres.Select(x => x.GenreId).ToList(),
                    ActorIds = serie.SerieActors.Select(x => x.ActorId).ToList()
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching series details", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Series ID" });

                var serie = await _unitOfWork.Repository<Serie>().Entities
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

                if (serie == null)
                    return NotFound(new { message = "Series not found" });

                var follows = await _unitOfWork.Repository<Follow>().Entities
                    .Where(f => f.SerieId == serie.Id)
                    .ToListAsync();

                foreach (var follow in follows)
                {
                    await _unitOfWork.Repository<Follow>().DeleteAsync(follow);
                }

                var ratings = await _unitOfWork.Repository<Review>().Entities
                    .Where(f => f.SerieId == serie.Id)
                    .ToListAsync();

                foreach (var rating in ratings)
                {
                    await _unitOfWork.Repository<Review>().DeleteAsync(rating);
                }

                await _unitOfWork.Repository<Serie>().DeleteAsync(serie);
                await _unitOfWork.Save(CancellationToken.None);

                return Ok(new { message = "Series deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the series", error = ex.Message });
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> GetEditSerie(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Series ID" });

                var serie = await _unitOfWork.Repository<Serie>().Entities
                    .FirstOrDefaultAsync(m => m.Id.Equals(id));

                if (serie == null)
                    return NotFound(new { message = "Series not found" });

                var model = new EditSerieModel
                {
                    Id = serie.Id,
                    Title = serie.Title,
                    About = serie.About,
                    AgeCategoryId = serie.AgeCategoryId,
                    PosterUrl = serie.Poster,
                    ProductionYear = serie.ProductionYear,
                    CountryIds = await _dbContext.SerieCountries
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.CountryId)
                        .ToListAsync(),
                    GenreIds = await _dbContext.SerieGenres
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.GenreId)
                        .ToListAsync(),
                    ActorIds = await _dbContext.SerieActors
                        .Where(x => x.SerieId == serie.Id)
                        .Select(x => x.ActorId)
                        .ToListAsync()
                };

                var metadata = await GetMetadata();
                return Ok(new { serie = model, metadata });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching series for edit", error = ex.Message });
            }
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] EditSerieModel update)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid request data", errors = ModelState });

                if (id == Guid.Empty || id != update.Id)
                    return BadRequest(new { message = "Invalid Series ID" });

                var serie = await _unitOfWork.Repository<Serie>().GetByIdAsync(id);
                if (serie == null)
                    return NotFound(new { message = "Series not found" });

                serie.Title = update.Title;
                serie.About = update.About;
                serie.AgeCategoryId = update.AgeCategoryId;
                serie.ProductionYear = update.ProductionYear;
                serie.CreatedAt = DateTime.UtcNow;

                if (update.Poster != null)
                {
                    var posterUri = await _storage.UploadFileAsync("poster", update.Poster);
                    serie.Poster = posterUri.ToString();
                }
                else
                {
                    serie.Poster = update.PosterUrl;
                }

                 _dbContext.Set<SerieActor>().RemoveRange(
                    _dbContext.Set<SerieActor>().Where(sa => sa.SerieId == serie.Id));
                 _dbContext.Set<SerieGenre>().RemoveRange(
                    _dbContext.Set<SerieGenre>().Where(sg => sg.SerieId == serie.Id));
                 _dbContext.Set<SerieCountry>().RemoveRange(
                    _dbContext.Set<SerieCountry>().Where(sc => sc.SerieId == serie.Id));

                if (update.ActorIds != null)
                {
                    foreach (var actorId in update.ActorIds)
                    {
                        _dbContext.Set<SerieActor>().Add(new SerieActor { SerieId = serie.Id, ActorId = actorId });
                    }
                }

                if (update.GenreIds != null)
                {
                    foreach (var genreId in update.GenreIds)
                    {
                        _dbContext.Set<SerieGenre>().Add(new SerieGenre { SerieId = serie.Id, GenreId = genreId });
                    }
                }

                if (update.CountryIds != null)
                {
                    foreach (var countryId in update.CountryIds)
                    {
                        _dbContext.Set<SerieCountry>().Add(new SerieCountry { SerieId = serie.Id, CountryId = countryId });
                    }
                }

                await _dbContext.SaveChangesAsync();
                await _unitOfWork.Save(CancellationToken.None);

                return Ok(new { message = "Series updated successfully", serieId = serie.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the series", error = ex.Message });
            }
        }

        [HttpGet("create")]
        public async Task<IActionResult> GetCreateSerie()
        {
            try
            {
                var metadata = await GetMetadata();
                return Ok(new { metadata });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching metadata for series creation", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSerieModel serie)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid Possessive invalid request data", errors = ModelState });

                var posterUri = serie.Poster != null
                    ? await _storage.UploadFileAsync("poster", serie.Poster)
                    : null;

                var newSerie = new Serie
                {
                    Id = Guid.NewGuid(),
                    Title = serie.Title,
                    About = serie.About,
                    AgeCategoryId = serie.AgeCategoryId,
                    Poster = posterUri?.ToString() ?? string.Empty,
                    ProductionYear = serie.ProductionYear,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Serie>().AddAsync(newSerie);

                if (serie.ActorIds != null)
                {
                    foreach (var actorId in serie.ActorIds)
                    {
                        _dbContext.Set<SerieActor>().Add(new SerieActor { SerieId = newSerie.Id, ActorId = actorId });
                    }
                }

                if (serie.GenreIds != null)
                {
                    foreach (var genreId in serie.GenreIds)
                    {
                        _dbContext.Set<SerieGenre>().Add(new SerieGenre { SerieId = newSerie.Id, GenreId = genreId });
                    }
                }

                if (serie.CountryIds != null)
                {
                    foreach (var countryId in serie.CountryIds)
                    {
                        _dbContext.Set<SerieCountry>().Add(new SerieCountry { SerieId = newSerie.Id, CountryId = countryId });
                    }
                }

                await _dbContext.SaveChangesAsync();
                await _unitOfWork.Save(CancellationToken.None);

                return Ok(new { message = "Series created successfully", serieId = newSerie.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the series", error = ex.Message });
            }
        }

        private async Task<object> GetMetadata()
        {
            var actors = await _unitOfWork.Repository<Actor>().GetAllAsync() ?? new List<Actor>();
            var genres = await _unitOfWork.Repository<Genre>().GetAllAsync() ?? new List<Genre>();
            var countries = await _unitOfWork.Repository<Country>().GetAllAsync() ?? new List<Country>();
            var ageCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync() ?? new List<AgeCategory>();

            return new
            {
                actors = actors.Select(a => new { a.Id, a.Name }),
                genres = genres.Select(g => new { g.Id, g.Name }),
                countries = countries.Select(c => new { c.Id, c.Name }),
                ageCategories = ageCategories.Select(ac => new { ac.Id, ac.Name })
            };
        }
    }
}