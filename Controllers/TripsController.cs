using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rental.Models;

namespace rental.Controllers
{
    public class TripsController : Controller
    {
        private readonly ModelContext _context;

        public TripsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            var trips = await _context.Trips
                .Include(t => t.CarIdfkNavigation)
                .Include(t => t.UserIdfkNavigation)
                .ToListAsync();

            return View(trips);
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.CarIdfkNavigation)
                .Include(t => t.UserIdfkNavigation)
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        public IActionResult Create()
        {
            ViewData["CarIdfk"] = new SelectList(_context.Cars, "CarId", "CarName");
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId");
            return View();
        }

        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TripId,StartTime,EndTime,LocationPickup,LocationDrop,UserIdfk,CarIdfk,Status")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown lists on error
            ViewData["CarIdfk"] = new SelectList(_context.Cars, "CarId", "CarName", trip.CarIdfk);
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", trip.UserIdfk);
            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            ViewData["CarIdfk"] = new SelectList(_context.Cars, "CarId", "CarName", trip.CarIdfk);
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", trip.UserIdfk);
            return View(trip);
        }

        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TripId,StartTime,EndTime,LocationPickup,LocationDrop,UserIdfk,CarIdfk,Status")] Trip trip)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown lists on error
            ViewData["CarIdfk"] = new SelectList(_context.Cars, "CarId", "CarName", trip.CarIdfk);
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", trip.UserIdfk);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.CarIdfkNavigation)
                .Include(t => t.UserIdfkNavigation)
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(decimal id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}
