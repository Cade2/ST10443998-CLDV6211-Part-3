namespace ST10443998_CLDV6211_POE.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Payment
    {
        public int PaymentId { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Payment amount is required")]
        [Range(0.01, 100000000000000, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }


        [Display(Name = "Payment Date")]
        [Required(ErrorMessage = "Payment date is required")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        // ✅ THIS IS IMPORTANT
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }
    }

}
