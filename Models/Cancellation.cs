using System.ComponentModel.DataAnnotations;

namespace Cozy.Models
{
    public class Cancellation
    {
        [Key]
        public int CancellationID { get; set; }

        [Required]
        public int BookingID { get; set; } // Foreign key

        public string? CancellationReason { get; set; }

        [Required]
        public DateTime CancellationDate { get; set; }

        [Required]
        public decimal RefundAmount { get; set; }

        [Required]
        public string? Status { get; set; } // "Requested", "Processed"

        // Navigation properties
        public virtual Booking? Booking { get; set; }
    }
}
