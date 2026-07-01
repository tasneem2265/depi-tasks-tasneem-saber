using CinemaBooking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Movies
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _context.Movies
                .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Title.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId.Value));

            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;

            var movies = await query.OrderBy(m => m.Title).ToListAsync();
            return View(movies);
        }

        // GET: /Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Category)
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.Cinema)
                .Include(m => m.Showtimes)
                    .ThenInclude(s => s.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var upcomingShowtimes = movie.Showtimes
                .Where(s => s.StartTime > DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToList();

            ViewBag.UpcomingShowtimes = upcomingShowtimes;

            return View(movie);
        }
    }
}