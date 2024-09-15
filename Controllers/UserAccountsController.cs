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
    public class UserAccountsController : Controller
    {
        private readonly ModelContext _context;

        public UserAccountsController(ModelContext context)
        {
            _context = context;
        }

        // GET: UserAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserAccounts.ToListAsync());
        }

        // GET: UserAccounts/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // GET: UserAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,Phone,Email,Password,City,Gender,Role,ImageFile")] UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.Email == userAccount.Email);

                if (existingUser != null)
                {
                    // If email exists, return validation error
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(userAccount);
                }

                if (userAccount.ImageFile != null && userAccount.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(userAccount.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userAccount.ImageFile.CopyToAsync(stream);
                    }

                    // Save file name to the UserAccount model
                    userAccount.UserImage = fileName;
                }

                _context.Add(userAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }


        // GET: UserAccounts/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        // POST: UserAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("UserId,UserName,Phone,Email,Password,City,Gender,Role,UserImage,ImageFile")] UserAccount userAccount)
        {
            if (id != userAccount.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
              
                try
                {
                    var existingUserAccount = await _context.UserAccounts.FindAsync(id);

                    if (userAccount.ImageFile != null && userAccount.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(userAccount.ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await userAccount.ImageFile.CopyToAsync(stream);
                        }

                        // Save file name to the UserAccount model
                        existingUserAccount.UserImage = fileName;
                    }

                    // Update other properties
                    existingUserAccount.UserName = userAccount.UserName;
                    existingUserAccount.Phone = userAccount.Phone;
                    existingUserAccount.Email = userAccount.Email;
                    existingUserAccount.Password = userAccount.Password;
                    existingUserAccount.City = userAccount.City;
                    existingUserAccount.Gender = userAccount.Gender;
                    existingUserAccount.Role = userAccount.Role;

                    _context.Update(existingUserAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.UserId))
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
            return View(userAccount);
        }

        // GET: UserAccounts/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount != null)
            {
                _context.UserAccounts.Remove(userAccount);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(decimal id)
        {
            return _context.UserAccounts.Any(e => e.UserId == id);
        }
    }
}
