using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using rental.Models;
using Microsoft.EntityFrameworkCore;

namespace rental.Controllers
{
    public class LogRegController : Controller
    {
        private readonly ModelContext _context;

        public LogRegController(ModelContext context)
        {
            _context = context;
        }

        // GET: LogReg/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: LogReg/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Set user session
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                // Role-based redirection
                switch (user.Role)
                {
                    case "Admin":
                        return RedirectToAction("Index", "Admin");
                    case "Driver":
                        return RedirectToAction("Driver", "Driver");
                    case "User":
                        return RedirectToAction("Index", "Home"); // Handle both Driver and User roles here
                    default:
                        ModelState.AddModelError("", "Invalid role assigned to user.");
                        return View();
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        // GET: LogReg/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: LogReg/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists in the database
                var existingUser = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.Email == userAccount.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(userAccount);
                }

                // If email is unique, proceed with registration
                _context.Add(userAccount);
                await _context.SaveChangesAsync();

                // Set session after registration
                HttpContext.Session.SetString("UserId", userAccount.UserId.ToString());
                HttpContext.Session.SetString("UserRole", userAccount.Role);

                return RedirectToAction("Login");
            }
            return View(userAccount);
        }
        // GET: LogReg/Logout
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
