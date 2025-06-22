using Microsoft.AspNetCore.Mvc;
using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;

        public PaymentsController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var payments = _db.Payments.ToList();
            return View(payments);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Customer", "Booking");
        }

        public IActionResult Edit(int id) // id = CustomerId
        {
            var booking = _db.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Payment.PaymentId == id);



            if (booking == null)
                return NotFound();

            return RedirectToAction("Edit", "Booking", new { id = booking.BookingId });
        }

        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Payment.PaymentId == id);

            if (booking == null)
                return NotFound();

            TempData["WarningMessage"] = "Deleting this customer will delete the full booking.";
            return RedirectToAction("Delete", "Booking", new { id = booking.BookingId });
        }
    }
}
