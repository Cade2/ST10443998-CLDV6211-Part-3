namespace ST10443998_CLDV6211_POE.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event
    {
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; }

        [Display(Name = "Event Date")]
        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Display(Name = "Event Time")]
        [Required(ErrorMessage = "Event time is required")]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Event Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid event type")]
        public int EventTypeId { get; set; }

        public EventType EventType { get; set; }

        [Display(Name = "Venue")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid venue")]
        public int VenueId { get; set; }

        public Venue Venue { get; set; }


        public ICollection<Booking> Bookings { get; set; }

    }

}
