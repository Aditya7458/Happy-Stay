using System.ComponentModel.DataAnnotations;

namespace Cozy.Models
{
    public class Room
        {
            [Key]
            public int RoomID { get; set; }

            [Required]
            public int HotelID { get; set; } // Foreign key

            [Required]
            [StringLength(50)]
            public string? RoomSize { get; set; }

            [Required]
            public string? BedType { get; set; } // "Single", "Double", "King"

            [Required]
            public int MaxOccupancy { get; set; }

            [Required]
            public decimal BaseFare { get; set; }

            public bool IsAC { get; set; } = false;

            public bool AvailabilityStatus { get; set; } = true;

            public int CreatedBy { get; set; } // Admin User ID

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation properties
            public virtual Hotel? Hotel { get; set; }
            public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        }
    }
