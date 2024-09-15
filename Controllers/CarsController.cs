using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rental.Models;

namespace rental.Controllers
{
    public class CarsController : Controller
    {
        private readonly ModelContext _context;

        public CarsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarId == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarId,CarNumber,CarName,CarColor,CarSet,CarValue2,CarValue3,ImageFile")] Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.ImageFile != null && car.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(car.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await car.ImageFile.CopyToAsync(stream);
                    }

                    // Save file name to the Car model
                    car.Imagepath = fileName;
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("CarId,CarNumber,CarName,CarColor,CarSet,ImagePath,CarValue2,CarValue3,ImageFile")] Car car)
        {
            if (id != car.CarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCar = await _context.Cars.FindAsync(id);

                    if (car.ImageFile != null && car.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(car.ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await car.ImageFile.CopyToAsync(stream);
                        }

                        // Save file name to the Car model
                        existingCar.Imagepath = fileName;
                    }

                    // Update other properties
                    existingCar.CarNumber = car.CarNumber;
                    existingCar.CarName = car.CarName;
                    existingCar.CarColor = car.CarColor;
                    existingCar.CarSet = car.CarSet;
                    existingCar.CarValue2 = car.CarValue2;
                    existingCar.CarValue3 = car.CarValue3;

                    _context.Update(existingCar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarId))
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
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarId == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(decimal id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }
    }
}
