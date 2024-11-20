using System.ComponentModel.DataAnnotations;


namespace Cozy.Models
{
    public class Booking
    {
            [Key]
            public int BookingID { get; set; }

            [Required]
            public int UserID { get; set; } // Foreign key

            [Required]
            public int RoomID { get; set; } // Foreign key

            [Required]
            public DateTime CheckInDate { get; set; }

            [Required]
            public DateTime CheckOutDate { get; set; }

            [Required]
            public decimal TotalPrice { get; set; }

            [Required]
            public int NumberOfAdults { get; set; }

            public int NumberOfChildren { get; set; }

            [Required]
            public string? Status { get; set; } // "Booked", "Completed"

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation properties
            public virtual User? User { get; set; }
            public virtual Room? Room { get; set; }
            public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
            public virtual Payment? Payment { get; set; }
        }
    }
