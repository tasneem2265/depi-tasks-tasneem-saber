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
    public class ShowtimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowtimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Cinema)
                .Include(s => s.Bookings)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
            return View(showtimes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new ShowtimeFormViewModel
            {
                AllMovies = await _context.Movies.OrderBy(m => m.Title).ToListAsync(),
                AllHalls = await _context.Halls.Include(h => h.Cinema).OrderBy(h => h.Cinema.Name).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShowtimeFormViewModel model)
        {
            model.AllMovies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();
            model.AllHalls = await _context.Halls.Include(h => h.Cinema).OrderBy(h => h.Cinema.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            if (model.StartTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.StartTime), "Start time must be in the future.");
                return View(model);
            }

            // Check no conflicting showtime in same hall
            var conflict = await _context.Showtimes
                .Include(s => s.Movie)
                .AnyAsync(s => s.HallId == model.HallId &&
                               s.StartTime < model.StartTime.AddMinutes(
                                   (await _context.Movies.FindAsync(model.MovieId))!.DurationMinutes) &&
                               model.StartTime < s.StartTime.AddMinutes(s.Movie.DurationMinutes));

            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "This hall already has a conflicting showtime in the same time slot.");
                return View(model);
            }

            var showtime = new Showtime
            {
                MovieId = model.MovieId,
                HallId = model.HallId,
                StartTime = model.StartTime,
                TicketPrice = model.TicketPrice
            };

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Showtime created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            return View(new ShowtimeFormViewModel
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                HallId = showtime.HallId,
                StartTime = showtime.StartTime,
                TicketPrice = showtime.TicketPrice,
                AllMovies = await _context.Movies.OrderBy(m => m.Title).ToListAsync(),
                AllHalls = await _context.Halls.Include(h => h.Cinema).OrderBy(h => h.Cinema.Name).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShowtimeFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            model.AllMovies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();
            model.AllHalls = await _context.Halls.Include(h => h.Cinema).OrderBy(h => h.Cinema.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            if (model.StartTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.StartTime), "Start time must be in the future.");
                return View(model);
            }

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            showtime.MovieId = model.MovieId;
            showtime.HallId = model.HallId;
            showtime.StartTime = model.StartTime;
            showtime.TicketPrice = model.TicketPrice;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Showtime updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null) return NotFound();
            return View(showtime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null) return NotFound();

            if (showtime.Bookings.Any(b => b.Status == Models.BookingStatus.Confirmed))
            {
                TempData["Error"] = "Cannot delete a showtime that has confirmed bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Showtime deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}