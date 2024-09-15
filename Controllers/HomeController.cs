using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rental.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace rental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;

        public HomeController(ILogger<HomeController> logger, ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve user email from session
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login if no user is logged in
                return RedirectToAction("Login", "LogReg");
            }

            // Fetch the user profile based on the email
            var userProfile = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (userProfile == null)
            {
                return NotFound();
            }


            // Populate dropdown lists for car types and user accounts
            ViewBag.CarIdfk = new SelectList(await _context.Cars.ToListAsync(), "CarId", "CarName");
            ViewBag.UserIdfk = new SelectList(await _context.UserAccounts.ToListAsync(), "UserId", "UserId");

            // Fetch existing trips (if any) to show in the view
            ViewBag.Trips = await _context.Trips.ToListAsync();

            // Fetch the team members (admins) to show in the view
            ViewBag.TeamMembers = await _context.UserAccounts
                .Where(u => u.Role == "Admin") // Example condition for team members
                .ToListAsync();

            var cars = await _context.Cars.ToListAsync();
            ViewBag.Cars = cars;

            // Pass user profile to the view
            ViewBag.UserProfile = userProfile;

            // Return the view
            return View();
        }


        // GET: /Home/Edit/5
        public async Task<IActionResult> Edit(decimal id)
        {
            // Fetch the trip to be edited
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Populate dropdown lists for car types and user accounts
            ViewData["CarIdfk"] = new SelectList(await _context.Cars.ToListAsync(), "CarId", "CarName", trip.CarIdfk);
            ViewData["UserIdfk"] = new SelectList(await _context.UserAccounts.ToListAsync(), "UserId", "UserId", trip.UserIdfk);

            // Return the view with the trip data
            return View(trip);
        }

        // POST: /Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TripId,StartTime,EndTime,LocationPickup,LocationDrop,UserIdfk,CarIdfk")] Trip trip, int StartTimeHour, int EndTimeHour)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update trip times with hours
                    trip.StartTime = DateTime.Parse($"{trip.StartTime:yyyy-MM-dd} {StartTimeHour}:00");
                    trip.EndTime = DateTime.Parse($"{trip.EndTime:yyyy-MM-dd} {EndTimeHour}:00");

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
            ViewData["CarIdfk"] = new SelectList(await _context.Cars.ToListAsync(), "CarId", "CarName", trip.CarIdfk);
            ViewData["UserIdfk"] = new SelectList(await _context.UserAccounts.ToListAsync(), "UserId", "UserId", trip.UserIdfk);

            // Return the view with the model if there were validation errors
            return View(trip);
        }

        public async Task<IActionResult> Reserve(string CarIdfk, string LocationPickup, string LocationDrop, DateTime PickupDate, int PickupHour, DateTime DropoffDate, int DropoffHour)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmail = HttpContext.Session.GetString("UserEmail");
                    if (string.IsNullOrEmpty(userEmail))
                    {
                        return RedirectToAction("Login", "LogReg");
                    }

                    var userProfile = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (userProfile == null)
                    {
                        return NotFound();
                    }

                    // Retrieve the car and its price
                    var car = await _context.Cars.FindAsync(Convert.ToDecimal(CarIdfk));
                    if (car == null)
                    {
                        return NotFound();
                    }

                    // Convert CarValue2 from string to decimal
                    if (!decimal.TryParse(car.CarValue2, out decimal carPrice))
                    {
                        ModelState.AddModelError("", "Invalid car price format.");
                        return View("Index"); // Adjust view as necessary
                    }

                    // Retrieve the user's credit card
                    var creditCard = await _context.Creditcards
                        .FirstOrDefaultAsync(c => c.Useracountfk == userProfile.UserId);

                    if (creditCard == null)
                    {
                        return NotFound(); // Handle case where no credit card is found
                    }

                    // Check if the credit card has enough balance
                    if (creditCard.Balance < carPrice)
                    {
                        ModelState.AddModelError("", "Insufficient credit card balance.");
                        return View("Index"); // Adjust view as necessary
                    }

                    // Deduct the car price from the credit card balance
                    creditCard.Balance -= carPrice;
                    _context.Update(creditCard);
                    await _context.SaveChangesAsync();

                    // Create a new trip and set the status to "Pending"
                    var trip = new Trip
                    {
                        CarIdfk = Convert.ToDecimal(CarIdfk),
                        LocationPickup = LocationPickup,
                        LocationDrop = LocationDrop,
                        StartTime = new DateTime(PickupDate.Year, PickupDate.Month, PickupDate.Day, PickupHour, 0, 0),
                        EndTime = new DateTime(DropoffDate.Year, DropoffDate.Month, DropoffDate.Day, DropoffHour, 0, 0),
                        UserIdfk = userProfile.UserId,
                        Status = "Pending" // Set status to "Pending"
                    };

                    _context.Trips.Add(trip);
                    await _context.SaveChangesAsync();

                    // Store trip and user information in the session
                    HttpContext.Session.SetString("LastBookedCar", CarIdfk);
                    HttpContext.Session.SetString("LastBookedLocationPickup", LocationPickup);
                    HttpContext.Session.SetString("LastBookedLocationDrop", LocationDrop);
                    HttpContext.Session.SetString("LastBookedPickupDate", PickupDate.ToString("yyyy-MM-dd"));
                    HttpContext.Session.SetInt32("LastBookedPickupHour", PickupHour);
                    HttpContext.Session.SetString("LastBookedDropoffDate", DropoffDate.ToString("yyyy-MM-dd"));
                    HttpContext.Session.SetInt32("LastBookedDropoffHour", DropoffHour);
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(userProfile.UserId));

                    TempData["SuccessMessage"] = "Your reservation has been successfully created!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while making the reservation.");
                    ModelState.AddModelError("", "An error occurred while processing your reservation. Please try again.");
                }
            }

            // Re-populate dropdowns on error and redisplay form
            ViewData["CarIdfk"] = new SelectList(await _context.Cars.ToListAsync(), "CarId", "CarName");
            return View("Index");
        }


        // GET: /Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: /Home/Contact
        public IActionResult Contact()
        {
            // Retrieve the success message from TempData, if available
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactU contactU)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    contactU.ContactDate = DateTime.Now; // Set current date/time for ContactDate
                    _context.ContactUs.Add(contactU); // Add the new contact to the DbSet
                    await _context.SaveChangesAsync(); // Save changes to the database

                    // Set success message and redirect
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while saving contact message.");
                    ModelState.AddModelError("", "An error occurred while sending your message. Please try again.");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(contactU);
        }

        // GET: /Home/Testimonial
        // GET: /Home/Testimonial
        public async Task<IActionResult> Testimonial()
        {
       

        var modelContext = _context.Testimonials.Include(t => t.UserIdfkNavigation);
            return View(await modelContext.ToListAsync());
        }

        // GET: /Home/Create
        // GET: /Home/Create
        public IActionResult Create()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg");
            }

            return View();
        }

        // POST: /Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Message,Status,CreatDate")] Testimonial testimonial)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg");
            }

            var userProfile = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (userProfile == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                testimonial.UserName = userProfile.UserName; // Set the username
                testimonial.ImagePath = userProfile.UserImage; // Set the user's image path

                testimonial.UserIdfk = userProfile.UserId;

                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Testimonial));
            }

            return View(testimonial);
        }
        // GET: /Home/Service
        public IActionResult Service()
        {
            return View();
        }




        // GET: /Home/UserProfiles
        public async Task<IActionResult> UserProfiles()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "LogReg"); // Redirect to login if no user is logged in
            }

            // Fetch the user profile based on the email
            var userProfile = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (userProfile == null)
            {
                return NotFound();
            }

            // Fetch the trips associated with the user
            var userTrips = await _context.Trips
        .Include(t => t.CarIdfkNavigation)
        .Where(t => t.UserIdfk == userProfile.UserId && t.Status == "Accepted")
        .ToListAsync();

            // Pass the user profile and trips to the view
            ViewBag.UserTrips = userTrips;
            return View(userProfile);
        }
        // GET: Home/EditUser/5
        public async Task<IActionResult> Edituser(decimal? id)
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

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edituser(decimal id, [Bind("UserId,UserName,Phone,Email,City,Gender,UserImage,ImageFile")] UserAccount userAccount)
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

                    if (existingUserAccount == null)
                    {
                        return NotFound();
                    }

                    // Handle file upload
                    if (userAccount.ImageFile != null && userAccount.ImageFile.Length > 0)
                    {
                        // Ensure the directory exists
                        var imgDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                        if (!Directory.Exists(imgDirectory))
                        {
                            Directory.CreateDirectory(imgDirectory);
                        }

                        var fileName = Path.GetFileName(userAccount.ImageFile.FileName);
                        var filePath = Path.Combine(imgDirectory, fileName);

                        // Handle existing file names by appending a unique identifier
                        if (System.IO.File.Exists(filePath))
                        {
                            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                            filePath = Path.Combine(imgDirectory, uniqueFileName);
                            fileName = uniqueFileName;
                        }

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await userAccount.ImageFile.CopyToAsync(stream);
                        }

                        // Update the user image in the database
                        existingUserAccount.UserImage = fileName;
                    }

                    // Update other properties
                    existingUserAccount.UserName = userAccount.UserName;
                    existingUserAccount.Phone = userAccount.Phone;
                    existingUserAccount.Email = userAccount.Email;
                    existingUserAccount.City = userAccount.City;
                    existingUserAccount.Gender = userAccount.Gender;

                    // Update the user account in the database
                    _context.Update(existingUserAccount);
                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        // Set success message
                        TempData["SuccessMessage"] = "Your profile has been updated successfully!";
                    }
                    else
                    {
                        // Log a warning or error, indicating that no rows were updated
                        ModelState.AddModelError("", "Failed to save changes. Please try again.");
                    }
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
            }
            else
            {
                // Handle validation errors
                ModelState.AddModelError("", "Please correct the errors and try again.");
            }

            return RedirectToAction(nameof(UserProfiles));
        }


        private bool UserAccountExists(decimal id)
        {
            return _context.UserAccounts.Any(e => e.UserId == id);
        }



        public async Task<IActionResult> Car()
        {
            // Fetch the list of cars from the database
            var cars = await _context.Cars.ToListAsync();

            // Pass the list of cars to the view
            return View(cars);
        }

        // GET: /Home/Team
        public async Task<IActionResult> Team()
        {
            // Fetch only the users who are part of the admin team
            var teamMembers = await _context.UserAccounts
                .Where(u => u.Role == "Admin") // Example condition
                .ToListAsync();

            return View(teamMembers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool TripExists(decimal id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
        public async Task<IActionResult> Creditcard()
        {
            // Retrieve user email from session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // If no user is logged in, redirect to login page
                return RedirectToAction("Login", "LogReg");
            }

            // Fetch user information based on email
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                // If user is not found, redirect to login page
                return RedirectToAction("Login", "LogReg");
            }

            // Fetch credit cards associated with the user
            var creditcards = _context.Creditcards
                .Where(c => c.Useracountfk == user.UserId)
                .Include(c => c.UseracountfkNavigation);

            return View(await creditcards.ToListAsync());
        }


    }
}
