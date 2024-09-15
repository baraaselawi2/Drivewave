using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rental.Models;
using Microsoft.AspNetCore.Http;

namespace rental.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;

        public AdminController(ModelContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg"); // Redirect to login if no user is logged in
            }

            // Fetch the user profile based on the email
            var userProfile = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Initialize ViewBag properties
            ViewBag.NumberOfCars = await _context.Cars.CountAsync();
            ViewBag.NumberOfUsers = await _context.UserAccounts.CountAsync();
            ViewBag.NumberOfTestimonials = await _context.Testimonials.CountAsync();
            ViewBag.NumberOfDrivers = await _context.UserAccounts
                .Where(u => u.Role == "Driver")  // Adjust the condition to match your role system
                .CountAsync();

            // Retrieve trips and related car data
            var trips = await _context.Trips
                .Include(t => t.CarIdfkNavigation)  // Ensure the related Car data is loaded
                .Where(t => t.Status != "Rejected")  // Exclude rejected trips
                .ToListAsync(); // Load data into memory

            // Compute total revenue
            var totalRevenue = trips.Sum(t =>
            {
                if (t.CarIdfkNavigation != null && !string.IsNullOrEmpty(t.CarIdfkNavigation.CarValue2))
                {
                    if (decimal.TryParse(t.CarIdfkNavigation.CarValue2, out decimal carPrice))
                    {
                        return carPrice;
                    }
                }
                return 0;
            });

            ViewBag.TotalRevenue = totalRevenue;

            // Compute total number of trips, excluding rejected trips
            ViewBag.NumberOfTrips = trips.Count();  // Since we've already filtered out rejected trips

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            // Retrieve the email of the logged-in user from the session
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg"); // Redirect to login if no user is logged in
            }

            // Fetch the user profile based on the email
            var userProfile = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Pass the user profile to the view
            return View(userProfile);
        }
    }
}
