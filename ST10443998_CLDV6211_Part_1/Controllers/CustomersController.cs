using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10443998_CLDV6211_POE.Controllers;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _db;

        public CustomersController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var Customers = _db.Customers.ToList();
            return View(Customers);
        }


        public IActionResult Create()
        {
            return RedirectToAction("Customer", "Booking");
        }

        public IActionResult Edit(int id) // id = CustomerId
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.Customer.CustomerId == id);

            if (booking == null)
                return NotFound();

            // ✅ Pass the correct BookingId here
            return RedirectToAction("Edit", "Booking", new { id = booking.BookingId });
        }

        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.Customer.CustomerId == id);

            if (booking == null)
                return NotFound();

            TempData["WarningMessage"] = "Deleting this customer will delete the full booking.";
            return RedirectToAction("Delete", "Booking", new { id = booking.BookingId });
        }


    }
}
