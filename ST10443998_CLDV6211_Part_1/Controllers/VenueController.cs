using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using ST10443998_CLDV6211_POE.Services;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly AzureBlobService _blobService;

        public VenueController(AppDbContext db, IConfiguration configuration, AzureBlobService blobService)
        {
            _db = db;
            _configuration = configuration;
            _blobService = blobService;
        }


        public IActionResult Index()
        {
            List<Venue> venues = _db.Venues.ToList();
            return View(venues);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            // REMOVE ModelState.IsValid check for testing
            // This will help confirm where the error really is

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageUrl = await _blobService.UploadImageAsync(imageFile);
                }

                _db.Venues.Add(venue);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log or show the error
                ViewBag.Error = ex.Message;
                return View(venue);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var venue = _db.Venues.Find(id);
            return View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Venue venue, IFormFile? imageFile)
        {
            var venueInDb = _db.Venues.FirstOrDefault(v => v.VenueId == venue.VenueId);
            if (venueInDb == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                var containerName = "venueimages";

                var containerClient = new BlobContainerClient(connectionString, containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(imageFile.FileName);

                using (var stream = imageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                venueInDb.ImageUrl = blobClient.Uri.ToString();
            }

            venueInDb.VenueName = venue.VenueName;
            venueInDb.Location = venue.Location;
            venueInDb.Capacity = venue.Capacity;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var venue = _db.Venues.FirstOrDefault(v => v.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: /Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var venue = _db.Venues
                .Include(v => v.Events)
                .ThenInclude(e => e.Bookings)
                .FirstOrDefault(v => v.VenueId == id);

            if (venue == null)
                return NotFound();

            bool hasActiveBookings = venue.Events.Any(e => e.Bookings != null && e.Bookings.Any());

            if (hasActiveBookings)
            {
                TempData["ErrorMessage"] = "You cannot delete this venue because it is linked to active bookings.";
                return RedirectToAction("Index");
            }

            _db.Venues.Remove(venue);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            var venue = _db.Venues.Find(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

    }
}
