using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _db;

        public BookingController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string searchString, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? isAvailable)
        {
            var bookings = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event).ThenInclude(e => e.Venue)
                .Include(b => b.Event).ThenInclude(e => e.EventType)
                .Include(b => b.Payment)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                bookings = bookings.Where(b =>
                    b.BookingId.ToString().Contains(searchString) ||
                    b.Event.EventName.ToLower().Contains(searchString));
            }

            if (eventTypeId.HasValue)
                bookings = bookings.Where(b => b.Event.EventTypeId == eventTypeId.Value);

            if (startDate.HasValue)
                bookings = bookings.Where(b => b.Event.EventDate >= startDate.Value);

            if (endDate.HasValue)
                bookings = bookings.Where(b => b.Event.EventDate <= endDate.Value);

            if (isAvailable.HasValue)
                bookings = bookings.Where(b => b.Event.Venue.IsAvailable == isAvailable.Value);

            ViewBag.EventTypes = _db.EventTypes.ToList();
            return View(bookings.ToList());
        }



        [HttpGet]
        public IActionResult Customer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Customer(Customer customer)
        {
            TempData["FullName"] = customer.FullName;
            TempData["Email"] = customer.Email;
            TempData["PhoneNumber"] = customer.PhoneNumber;

            return RedirectToAction("Event");
        }


        [HttpGet]
        public IActionResult Event(int customerId)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Event(Event ev)
        {
            bool isAlreadyBooked = _db.Bookings
                .Include(b => b.Event)
                .Any(b =>
                    b.Event.VenueId == ev.VenueId &&
                    b.Event.EventDate == ev.EventDate &&
                    b.Event.EventTime == ev.EventTime);

            if (isAlreadyBooked)
            {
                ViewBag.Venues = _db.Venues.ToList();
                ViewBag.EventTypes = _db.EventTypes.ToList();
                ViewBag.ErrorMessage = "This venue is already booked on that date and time.";
                return View(ev);
            }

            TempData["EventName"] = ev.EventName;
            TempData["Description"] = ev.Description;
            TempData["EventDate"] = ev.EventDate.ToString("o"); // ISO format for TimeSpan compatibility
            TempData["EventTime"] = ev.EventTime.ToString();
            TempData["EventTypeId"] = ev.EventTypeId;
            TempData["VenueId"] = ev.VenueId;

            return RedirectToAction("Payment");
        }


        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Payment(Payment payment)
        {
            // Reconstruct Customer
            var customer = new Customer
            {
                FullName = TempData["FullName"]?.ToString(),
                Email = TempData["Email"]?.ToString(),
                PhoneNumber = TempData["PhoneNumber"]?.ToString()
            };
            _db.Customers.Add(customer);
            _db.SaveChanges();

            // Reconstruct Event
            var eventDate = DateTime.Parse(TempData["EventDate"]?.ToString());
            var eventTime = TimeSpan.Parse(TempData["EventTime"]?.ToString());

            var ev = new Event
            {
                EventName = TempData["EventName"]?.ToString(),
                Description = TempData["Description"]?.ToString(),
                EventDate = eventDate,
                EventTime = eventTime,
                EventTypeId = int.Parse(TempData["EventTypeId"]?.ToString()),
                VenueId = int.Parse(TempData["VenueId"]?.ToString())
            };

            _db.Events.Add(ev);
            _db.SaveChanges();

            // Save booking
            var booking = new Booking
            {
                BookingDate = DateTime.Now,
                CustomerId = customer.CustomerId,
                Event = ev
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();

            // Save payment
            payment.BookingId = booking.BookingId;
            _db.Payments.Add(payment);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }




        [HttpGet]
        public IActionResult Details(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Event).ThenInclude(e => e.Venue)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) return NotFound();
            return View(booking);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Event.EventType)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();

            return View(booking);
        }


        [HttpPost]
        public IActionResult Edit(Booking booking)
        {
            var existingBooking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == booking.BookingId);

            if (existingBooking == null)
                return NotFound();

            // Update Booking
            existingBooking.BookingDate = booking.BookingDate;

            // Update Customer
            existingBooking.Customer.FullName = Request.Form["Customer.FullName"];
            existingBooking.Customer.Email = Request.Form["Customer.Email"];
            existingBooking.Customer.PhoneNumber = Request.Form["Customer.PhoneNumber"];

            // Update Event
            existingBooking.Event.EventName = Request.Form["Event.EventName"];
            existingBooking.Event.Description = Request.Form["Event.Description"];
            existingBooking.Event.EventDate = DateTime.Parse(Request.Form["Event.EventDate"]);
            existingBooking.Event.EventTypeId = int.Parse(Request.Form["Event.EventTypeId"]);
            existingBooking.Event.VenueId = int.Parse(Request.Form["Event.VenueId"]);

            // Update Payment
            existingBooking.Payment.Amount = decimal.Parse(Request.Form["Payment.Amount"]);
            existingBooking.Payment.PaymentDate = DateTime.Parse(Request.Form["Payment.PaymentDate"]);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking); // ✅ must point to a real view
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            // Delete the associated event
            if (booking.Event != null)
            {
                var eventWithBookings = _db.Events
                    .Include(e => e.Bookings)
                    .FirstOrDefault(e => e.EventId == booking.Event.EventId);

                if (eventWithBookings != null && eventWithBookings.Bookings.Any())
                {
                    TempData["ErrorMessage"] = "You cannot delete this event because it is linked to a booking.";
                    return RedirectToAction("Index");
                }

                _db.Events.Remove(eventWithBookings);
            }


            // Delete the associated payment
            if (booking.Payment != null)
                _db.Payments.Remove(booking.Payment);

            // Delete the associated customer
            if (booking.Customer != null)
                _db.Customers.Remove(booking.Customer);

            // Delete the booking itself
            _db.Bookings.Remove(booking);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

    }
}
