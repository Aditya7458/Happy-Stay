using System.ComponentModel.DataAnnotations;

public class AddHotelDTO
{
    [Required(ErrorMessage = "Hotel name is required.")]
    [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
    public string Location { get; set; }

    public string? Description { get; set; }

    public string? Amenities { get; set; } // JSON formatted string

    public string? ImageURL { get; set; }

    public bool IsActive { get; set; } = true;

    public int CreatedBy { get; set; } // Admin User ID
}
