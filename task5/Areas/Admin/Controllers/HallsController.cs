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
    public class HallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var halls = await _context.Halls
                .Include(h => h.Cinema)
                .OrderBy(h => h.Cinema.Name).ThenBy(h => h.Name)
                .ToListAsync();
            return View(halls);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new HallFormViewModel
            {
                AllCinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HallFormViewModel model)
        {
            model.AllCinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var hall = new Hall
            {
                Name = model.Name,
                SeatCapacity = model.SeatCapacity,
                CinemaId = model.CinemaId
            };

            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hall created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null) return NotFound();

            return View(new HallFormViewModel
            {
                Id = hall.Id,
                Name = hall.Name,
                SeatCapacity = hall.SeatCapacity,
                CinemaId = hall.CinemaId,
                AllCinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HallFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            model.AllCinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var hall = await _context.Halls.FindAsync(id);
            if (hall == null) return NotFound();

            hall.Name = model.Name;
            hall.SeatCapacity = model.SeatCapacity;
            hall.CinemaId = model.CinemaId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Hall updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var hall = await _context.Halls
                .Include(h => h.Cinema)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hall == null) return NotFound();
            return View(hall);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hall = await _context.Halls
                .Include(h => h.Showtimes)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hall == null) return NotFound();

            if (hall.Showtimes.Any())
            {
                TempData["Error"] = "Cannot delete this hall because it has showtimes. Remove its showtimes first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hall deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}