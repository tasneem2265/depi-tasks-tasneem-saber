using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View(movies);
        }

        // GET: /Admin/Movies/Create
        public async Task<IActionResult> Create()
        {
            var vm = new MovieFormViewModel
            {
                AllCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync()
            };
            return View(vm);
        }

        // POST: /Admin/Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            model.AllCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            string? posterPath = null;
            if (model.PosterFile != null)
            {
                var (ok, error, path) = await SavePosterAsync(model.PosterFile);
                if (!ok)
                {
                    ModelState.AddModelError(nameof(model.PosterFile), error!);
                    return View(model);
                }
                posterPath = path;
            }

            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                DurationMinutes = model.DurationMinutes,
                PosterPath = posterPath,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var catId in model.SelectedCategoryIds)
                movie.MovieCategories.Add(new MovieCategory { CategoryId = catId });

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var vm = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ExistingPosterPath = movie.PosterPath,
                SelectedCategoryIds = movie.MovieCategories.Select(mc => mc.CategoryId).ToList(),
                AllCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync()
            };

            return View(vm);
        }

        // POST: /Admin/Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieFormViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            model.AllCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var movie = await _context.Movies
                .Include(m => m.MovieCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            if (model.PosterFile != null)
            {
                var (ok, error, path) = await SavePosterAsync(model.PosterFile);
                if (!ok)
                {
                    ModelState.AddModelError(nameof(model.PosterFile), error!);
                    model.ExistingPosterPath = movie.PosterPath;
                    return View(model);
                }

                DeletePosterFile(movie.PosterPath);
                movie.PosterPath = path;
            }

            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.DurationMinutes = model.DurationMinutes;

            // Update categories
            movie.MovieCategories.Clear();
            foreach (var catId in model.SelectedCategoryIds)
                movie.MovieCategories.Add(new MovieCategory { MovieId = movie.Id, CategoryId = catId });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        // POST: /Admin/Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Showtimes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            if (movie.Showtimes.Any())
            {
                TempData["Error"] = "Cannot delete this movie because it has showtimes. Remove its showtimes first.";
                return RedirectToAction(nameof(Index));
            }

            DeletePosterFile(movie.PosterPath);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<(bool ok, string? error, string? path)> SavePosterAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(ext))
                return (false, "Only .jpg, .jpeg, .png, .webp files are allowed.", null);

            if (file.Length > MaxFileSizeBytes)
                return (false, "Poster file must not exceed 5 MB.", null);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posters");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, null, $"/uploads/posters/{fileName}");
        }

        private void DeletePosterFile(string? posterPath)
        {
            if (string.IsNullOrEmpty(posterPath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, posterPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}