using System.ComponentModel.DataAnnotations;

namespace Cozy.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int BookingID { get; set; } // Foreign key

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string? PaymentMethod { get; set; } // "CreditCard", "DebitCard", etc.

        [Required]
        public string? PaymentStatus { get; set; } // "Pending", "Completed", "Failed"

        public DateTime PaymentDate { get; set; }

        // Navigation properties
        public virtual Booking? Booking { get; set; }
    }
}
