using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rental.Models;

namespace rental.Controllers
{
    public class TestimonialsController : Controller
    {
        private readonly ModelContext _context;

        public TestimonialsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Testimonials
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Testimonials.Include(t => t.UserIdfkNavigation);
            return View(await modelContext.ToListAsync());
        }

        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.UserIdfkNavigation)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: Testimonials/Create
        public IActionResult Create()
        {
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId");
            return View();
        }

        // POST: Testimonials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,UserName,Message,Status,CreatDate,UserIdfk,ImageFile")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                if (testimonial.ImageFile != null && testimonial.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(testimonial.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await testimonial.ImageFile.CopyToAsync(stream);
                    }

                    // Save file name to the Testimonial model
                    testimonial.ImagePath = fileName;
                }

                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", testimonial.UserIdfk);
            return View(testimonial);
        }

        // GET: Testimonials/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", testimonial.UserIdfk);
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("CommentId,UserName,Message,Status,CreatDate,UserIdfk,ImagePath,ImageFile")] Testimonial testimonial)
        {
            if (id != testimonial.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTestimonial = await _context.Testimonials.FindAsync(id);

                    if (testimonial.ImageFile != null && testimonial.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(testimonial.ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await testimonial.ImageFile.CopyToAsync(stream);
                        }

                        // Save file name to the Testimonial model
                        existingTestimonial.ImagePath = fileName;
                    }
                    else
                    {
                        // Keep the existing image if no new image is uploaded
                        existingTestimonial.ImagePath = testimonial.ImagePath;
                    }

                    // Update other properties
                    existingTestimonial.UserName = testimonial.UserName;
                    existingTestimonial.Message = testimonial.Message;
                    existingTestimonial.Status = testimonial.Status;
                    existingTestimonial.CreatDate = testimonial.CreatDate;
                    existingTestimonial.UserIdfk = testimonial.UserIdfk;

                    _context.Update(existingTestimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.CommentId))
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
            ViewData["UserIdfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", testimonial.UserIdfk);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.UserIdfkNavigation)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(decimal id)
        {
            return _context.Testimonials.Any(e => e.CommentId == id);
        }
    }
}
