using CinemaBooking.Data;
using CinemaBooking.Models;
using CinemaBooking.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Bookings/Create?showtimeId=5
        [HttpGet]
        public async Task<IActionResult> Create(int showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Cinema)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                return NotFound();

            if (showtime.StartTime <= DateTime.UtcNow)
            {
                TempData["Error"] = "This showtime has already started and cannot be booked.";
                return RedirectToAction("Details", "Movies", new { id = showtime.MovieId });
            }

            if (showtime.SeatsAvailable <= 0)
            {
                TempData["Error"] = "Sorry, this showtime is sold out.";
                return RedirectToAction("Details", "Movies", new { id = showtime.MovieId });
            }

            var vm = new BookingCreateViewModel
            {
                ShowtimeId = showtime.Id,
                MovieTitle = showtime.Movie.Title,
                CinemaName = showtime.Hall.Cinema.Name,
                HallName = showtime.Hall.Name,
                StartTime = showtime.StartTime,
                TicketPrice = showtime.TicketPrice,
                SeatsAvailable = showtime.SeatsAvailable,
                SeatsCount = 1
            };

            return View(vm);
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Hall).ThenInclude(h => h.Cinema)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == model.ShowtimeId);

            if (showtime == null)
                return NotFound();

            // Re-populate display fields in case we need to redisplay the form
            model.MovieTitle = showtime.Movie.Title;
            model.CinemaName = showtime.Hall.Cinema.Name;
            model.HallName = showtime.Hall.Name;
            model.StartTime = showtime.StartTime;
            model.TicketPrice = showtime.TicketPrice;
            model.SeatsAvailable = showtime.SeatsAvailable;

            if (showtime.StartTime <= DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "This showtime has already started and cannot be booked.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            // Server-side capacity check (critical business rule)
            if (model.SeatsCount > showtime.SeatsAvailable)
            {
                ModelState.AddModelError(nameof(model.SeatsCount),
                    $"Only {showtime.SeatsAvailable} seat(s) left for this showtime.");
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Challenge();

            var booking = new Booking
            {
                UserId = userId,
                ShowtimeId = showtime.Id,
                SeatsCount = model.SeatsCount,
                TotalPrice = model.SeatsCount * showtime.TicketPrice,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Confirmed
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmed! Enjoy the movie.";
            return RedirectToAction(nameof(MyBookings));
        }

        // GET: /Bookings/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);

            var bookings = await _context.Bookings
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Hall).ThenInclude(h => h.Cinema)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // POST: /Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound();

            if (booking.Status == BookingStatus.Cancelled)
            {
                TempData["Error"] = "This booking is already cancelled.";
                return RedirectToAction(nameof(MyBookings));
            }

            if (booking.Showtime.StartTime <= DateTime.UtcNow)
            {
                TempData["Error"] = "Cannot cancel a booking after the showtime has started.";
                return RedirectToAction(nameof(MyBookings));
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(MyBookings));
        }
    }
}