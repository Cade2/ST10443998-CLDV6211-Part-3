using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10443998_CLDV6211_POE.Controllers;

namespace EventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var events = _db.Events.ToList();
            return View(events);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Customer", "Booking");
        }

        public IActionResult Edit(int id) // id = CustomerId
        {
            var booking = _db.Bookings
                .Include(b => b.Event)
                .FirstOrDefault(b => b.Event.EventId == id);


            if (booking == null)
                return NotFound();

            return RedirectToAction("Edit", "Booking", new { id = booking.BookingId });
        }

        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Event)
                .FirstOrDefault(b => b.Event.EventId == id);

            if (booking == null)
                return NotFound();

            TempData["WarningMessage"] = "Deleting this customer will delete the full booking.";
            return RedirectToAction("Delete", "Booking", new { id = booking.BookingId });
        }
    }
}
