using CinemaBooking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nowShowing = await _context.Movies
                .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Category)
                .Where(m => m.Showtimes.Any(s => s.StartTime > DateTime.UtcNow))
                .OrderByDescending(m => m.CreatedAt)
                .Take(8)
                .ToListAsync();

            return View(nowShowing);
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult StatusCode(int code)
        {
            ViewBag.StatusCode = code;
            if (code == 404)
                return View("NotFound");
            return View("Error");
        }
    }
}