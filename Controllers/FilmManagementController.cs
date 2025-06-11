using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netflex.Database;
using Netflex.Models.Film;
using OfficeOpenXml;
using X.PagedList.Extensions;

namespace Netflex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class FilmManagementController : ControllerBase
    {
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 6;

        public FilmManagementController(IStorageService storage, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _storage = storage;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetFilms(string? searchTerm, int? productionYear, string? sortOrder, int? page, bool export = false)
        {
            int pageNumber = page ?? 1;
            var query = _unitOfWork.Repository<Film>().Entities.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.Title.ToLower().Contains(searchTerm.ToLower()));
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

            if (export)
            {
                return ExportToExcel(searchTerm, productionYear, sortOrder);
            }

            var models = query.Select(film => new FilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear
            }).ToPagedList(pageNumber, PAGE_SIZE);

            return Ok(new
            {
                Films = models,
                SearchTerm = searchTerm,
                ProductionYear = productionYear,
                SortOrder = sortOrder
            });
        }

        [HttpGet("export")]
        public IActionResult ExportToExcel(string? searchTerm, int? productionYear, string? sortOrder)
        {
            var query = _unitOfWork.Repository<Film>().Entities.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.Title.Contains(searchTerm));
            }

            if (productionYear.HasValue)
            {
                query = query.Where(f => f.ProductionYear == productionYear);
            }

            query = sortOrder switch
            {
                "title" => query.OrderBy(f => f.Title),
                "title_desc" => query.OrderByDescending(f => f.Title),
                "production_year" => query.OrderBy(f => f.ProductionYear),
                "production_year_desc" => query.OrderByDescending(f => f.ProductionYear),
                _ => query.OrderBy(f => f.Title)
            };

            var films = query.Select(f => new FilmViewModel
            {
                Id = f.Id,
                Title = f.Title,
                Poster = f.Poster,
                Trailer = f.Trailer,
                ProductionYear = f.ProductionYear
            }).ToList();

            if (!films.Any())
            {
                return BadRequest("No films found for export.");
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Films");

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Title";
            worksheet.Cells[1, 3].Value = "Production Year";
            worksheet.Cells[1, 4].Value = "Trailer Link";
            worksheet.Cells[1, 5].Value = "Poster";
            worksheet.Cells[1, 6].Value = "Path";

            for (int i = 0; i < films.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = films[i].Id;
                worksheet.Cells[i + 2, 2].Value = films[i].Title;
                worksheet.Cells[i + 2, 3].Value = films[i].ProductionYear;
                worksheet.Cells[i + 2, 4].Value = films[i].Trailer;
                worksheet.Cells[i + 2, 5].Value = films[i].Poster;
                worksheet.Cells[i + 2, 6].Value = films[i].Path;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Films-{Guid.NewGuid()}.xlsx");
        }

        [HttpGet("{id}")]
        public IActionResult GetFilmDetail(Guid id)
        {
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
            {
                return NotFound("Film not found");
            }

            var model = new DetailFilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                About = film.About,
                Poster = film.Poster,
                Path = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear
            };

            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
            {
                return NotFound("Film not found");
            }

            var followsToRemove = _unitOfWork.Repository<Follow>().Entities.Where(f => f.FilmId == id).ToList();
            foreach (var follow in followsToRemove)
            {
                await _unitOfWork.Repository<Follow>().DeleteAsync(follow);
            }

            var ratings = _unitOfWork.Repository<Review>().Entities.Where(f => f.FilmId == film.Id).ToList();
            foreach (var rating in ratings)
            {
                await _unitOfWork.Repository<Review>().DeleteAsync(rating);
            }

            await _unitOfWork.Repository<Film>().DeleteAsync(film);
            await _unitOfWork.Save(CancellationToken.None);

            return NoContent();
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> GetFilmForEdit(Guid id)
        {
            var film = _unitOfWork.Repository<Film>().Entities.FirstOrDefault(m => m.Id.Equals(id));
            if (film == null)
            {
                return NotFound("Film not found");
            }

            var model = new EditFilmViewModel
            {
                Id = film.Id,
                Title = film.Title,
                About = film.About,
                PosterUrl = film.Poster,
                FileUrl = film.Path,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear,
                CountryIds = _context.FilmCountries.Where(x => x.FilmId == film.Id).Select(x => x.CountryId).ToList(),
                GenreIds = _context.FilmGenres.Where(x => x.FilmId == film.Id).Select(x => x.GenreId).ToList(),
                ActorIds = _context.FilmActors.Where(x => x.FilmId == film.Id).Select(x => x.ActorId).ToList(),
            };

            var actors = await _unitOfWork.Repository<Actor>().GetAllAsync() ?? new List<Actor>();
            var genres = await _unitOfWork.Repository<Genre>().GetAllAsync() ?? new List<Genre>();
            var countries = await _unitOfWork.Repository<Country>().GetAllAsync() ?? new List<Country>();
            var ageCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync() ?? new List<AgeCategory>();

            return Ok(new
            {
                Film = model,
                Actors = actors.Select(a => new { a.Id, a.Name }),
                Genres = genres.Select(g => new { g.Id, g.Name }),
                Countries = countries.Select(c => new { c.Id, c.Name }),
                AgeCategories = ageCategories.Select(ac => new { ac.Id, ac.Name })
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EditFilmViewModel update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var film = await _unitOfWork.Repository<Film>().GetByIdAsync(id);
            if (film == null)
            {
                return NotFound("Film not found");
            }

            var filmUri = update.File != null ? await _storage.UploadFileAsync("film", update.File) : null;
            film.Title = update.Title;
            film.About = update.About;
            film.AgeCategoryId = update.AgeCategoryId;
            film.ProductionYear = update.ProductionYear;
            film.CreatedAt = DateTime.UtcNow;
            film.Path = filmUri?.ToString() ?? update.FileUrl;
            film.Trailer = update.Trailer;

            if (update.Poster != null)
            {
                var posterUri = await _storage.UploadFileAsync("poster", update.Poster);
                film.Poster = posterUri.ToString();
            }
            else
            {
                film.Poster = update.PosterUrl;
            }

            _context.Set<FilmActor>().RemoveRange(_context.Set<FilmActor>().Where(sa => sa.FilmId == film.Id));
            _context.Set<FilmGenre>().RemoveRange(_context.Set<FilmGenre>().Where(sg => sg.FilmId == film.Id));
            _context.Set<FilmCountry>().RemoveRange(_context.Set<FilmCountry>().Where(sc => sc.FilmId == film.Id));

            if (update.ActorIds != null)
            {
                foreach (var actorId in update.ActorIds)
                {
                    _context.Set<FilmActor>().Add(new FilmActor { FilmId = film.Id, ActorId = actorId });
                }
            }

            if (update.GenreIds != null)
            {
                foreach (var genreId in update.GenreIds)
                {
                    _context.Set<FilmGenre>().Add(new FilmGenre { FilmId = film.Id, GenreId = genreId });
                }
            }

            if (update.CountryIds != null)
            {
                foreach (var countryId in update.CountryIds)
                {
                    _context.Set<FilmCountry>().Add(new FilmCountry { FilmId = film.Id, CountryId = countryId });
                }
            }

            await _context.SaveChangesAsync();
            await _unitOfWork.Save(CancellationToken.None);

            return Ok(film);
        }

        [HttpGet("create")]
        public async Task<IActionResult> GetCreateFilmData()
        {
            var actors = await _unitOfWork.Repository<Actor>().GetAllAsync() ?? new List<Actor>();
            var genres = await _unitOfWork.Repository<Genre>().GetAllAsync() ?? new List<Genre>();
            var countries = await _unitOfWork.Repository<Country>().GetAllAsync() ?? new List<Country>();
            var ageCategories = await _unitOfWork.Repository<AgeCategory>().GetAllAsync() ?? new List<AgeCategory>();

            return Ok(new
            {
                Actors = actors.Select(a => new { a.Id, a.Name }),
                Genres = genres.Select(g => new { g.Id, g.Name }),
                Countries = countries.Select(c => new { c.Id, c.Name }),
                AgeCategories = ageCategories.Select(ac => new { ac.Id, ac.Name })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateFilmViewModel film)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posterUri = film.Poster != null ? await _storage.UploadFileAsync("poster", film.Poster) : null;
            var filmUri = film.File != null ? await _storage.UploadFileAsync("film", film.File) : null;

            var newFilm = new Film
            {
                Id = Guid.NewGuid(),
                Title = film.Title,
                About = film.About,
                Poster = posterUri?.ToString() ?? string.Empty,
                Path = filmUri?.ToString() ?? string.Empty,
                Trailer = film.Trailer,
                ProductionYear = film.ProductionYear,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Film>().AddAsync(newFilm);
            await _unitOfWork.Save(CancellationToken.None);

            if (film.ActorIds != null)
            {
                var filmActors = film.ActorIds.Select(actorId => new FilmActor
                {
                    FilmId = newFilm.Id,
                    ActorId = actorId
                }).ToList();
                _context.Set<FilmActor>().AddRange(filmActors);
            }

            if (film.GenreIds != null)
            {
                var filmGenres = film.GenreIds.Select(genreId => new FilmGenre
                {
                    FilmId = newFilm.Id,
                    GenreId = genreId
                }).ToList();
                _context.Set<FilmGenre>().AddRange(filmGenres);
            }

            if (film.CountryIds != null)
            {
                var filmCountries = film.CountryIds.Select(countryId => new FilmCountry
                {
                    FilmId = newFilm.Id,
                    CountryId = countryId
                }).ToList();
                _context.Set<FilmCountry>().AddRange(filmCountries);
            }

            await _context.SaveChangesAsync();
            await _unitOfWork.Save(CancellationToken.None);

            return CreatedAtAction(nameof(GetFilmDetail), new { id = newFilm.Id }, newFilm.Id);
        }
    }
}