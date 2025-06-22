namespace ST10443998_CLDV6211_POE.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        public int CustomerId { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Customer name is required")]
        public string FullName { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Customer email is required")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Customer phone number is required")]
        public string PhoneNumber { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }

}
