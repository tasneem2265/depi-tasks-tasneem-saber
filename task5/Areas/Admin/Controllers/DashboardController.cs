using CinemaBooking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.MoviesCount = await _context.Movies.CountAsync();
            ViewBag.CinemasCount = await _context.Cinemas.CountAsync();
            ViewBag.HallsCount = await _context.Halls.CountAsync();
            ViewBag.ShowtimesCount = await _context.Showtimes.CountAsync(s => s.StartTime > DateTime.UtcNow);
            ViewBag.BookingsCount = await _context.Bookings.CountAsync(b => b.Status == Models.BookingStatus.Confirmed);

            return View();
        }
    }
}