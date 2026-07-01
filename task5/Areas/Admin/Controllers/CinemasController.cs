using CinemaBooking.Data;
using CinemaBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CinemasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.Halls)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create() => View(new Cinema());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Cinemas.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cinema created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            _context.Cinemas.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cinema updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Halls)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Halls)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null) return NotFound();

            if (cinema.Halls.Any())
            {
                TempData["Error"] = "Cannot delete this cinema because it has halls. Remove its halls first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cinema deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}