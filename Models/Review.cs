using System.ComponentModel.DataAnnotations;

namespace Cozy.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign key

        [Required]
        public int HotelID { get; set; } // Foreign key

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? ReviewText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Hotel? Hotel { get; set; }
    }
}
