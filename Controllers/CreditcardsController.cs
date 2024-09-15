using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using rental.Models;

namespace rental.Controllers
{
    public class CreditcardsController : Controller
    {
        private readonly ModelContext _context;

        public CreditcardsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Creditcards
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Creditcards.Include(c => c.UseracountfkNavigation);
            return View(await modelContext.ToListAsync());
        }

        // GET: Creditcards/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards
                .Include(c => c.UseracountfkNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditcard == null)
            {
                return NotFound();
            }

            return View(creditcard);
        }

        // GET: Creditcards/Create
        public IActionResult Create()
        {
            ViewData["Useracountfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId");
            return View();
        }

        // POST: Creditcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Cvv,Cardnumber,Expiredate,Balance,Email,Useracountfk")] Creditcard creditcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(creditcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Useracountfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", creditcard.Useracountfk);
            return View(creditcard);
        }

        // GET: Creditcards/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards.FindAsync(id);
            if (creditcard == null)
            {
                return NotFound();
            }
            ViewData["Useracountfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", creditcard.Useracountfk);
            return View(creditcard);
        }

        // POST: Creditcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name,Cvv,Cardnumber,Expiredate,Balance,Email,Useracountfk")] Creditcard creditcard)
        {
            if (id != creditcard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditcard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditcardExists(creditcard.Id))
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
            ViewData["Useracountfk"] = new SelectList(_context.UserAccounts, "UserId", "UserId", creditcard.Useracountfk);
            return View(creditcard);
        }

        // GET: Creditcards/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards
                .Include(c => c.UseracountfkNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditcard == null)
            {
                return NotFound();
            }

            return View(creditcard);
        }

        // POST: Creditcards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var creditcard = await _context.Creditcards.FindAsync(id);
            if (creditcard != null)
            {
                _context.Creditcards.Remove(creditcard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditcardExists(decimal id)
        {
            return _context.Creditcards.Any(e => e.Id == id);
        }
    }
}
