using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;


namespace ST10443998_CLDV6211_POE.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        public string VenueName { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
        public int Capacity { get; set; }
        [Display(Name = "Available?")]
        public bool IsAvailable { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
