using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rental.Models;
using Microsoft.AspNetCore.Http;


namespace rental.Controllers
{
    public class DriverController : Controller
    {
        private readonly ModelContext _context;

        public DriverController(ModelContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Driver()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg");
            }

            var userProfile = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Retrieve statistics
            ViewBag.NumberOfCars = await _context.Cars.CountAsync();
            ViewBag.NumberOfTrips = await _context.Trips.CountAsync();
            ViewBag.NumberOfUsers = await _context.UserAccounts.CountAsync();
            ViewBag.NumberOfTestimonials = await _context.Testimonials.CountAsync();
            ViewBag.NumberOfDrivers = await _context.UserAccounts
                .Where(u => u.Role == "Driver")
                .CountAsync();

            // Calculate total revenue from accepted trips
            var acceptedTrips = await _context.Trips
                .Include(t => t.CarIdfkNavigation)
                .Include(t => t.UserIdfkNavigation)// Ensure the related Car data is loaded
                .Where(t => t.Status == "Accepted") // Filter only accepted trips
                .ToListAsync(); // Load data into memory

            var totalRevenue = acceptedTrips.Sum(t =>
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
            ViewBag.acceptedTrips=acceptedTrips;

            ViewBag.TotalRevenue = totalRevenue;
            
            // Count the number of accepted trips
            ViewBag.NumberOfAcceptedTrips = acceptedTrips.Count;

            // Retrieve pending trips for this driver
            var pendingTrips = await _context.Trips
                .Include(t => t.CarIdfkNavigation)
                .Include(t => t.UserIdfkNavigation)
                .Where(t => t.Status == "Pending")
                .ToListAsync();
         

            return View(pendingTrips);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptTrip(decimal id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            trip.Status = "Accepted";
            _context.Update(trip);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Driver));
        }

        [HttpPost]
        public async Task<IActionResult> RejectTrip(decimal id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            trip.Status = "Rejected";
            _context.Update(trip);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Driver));
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
