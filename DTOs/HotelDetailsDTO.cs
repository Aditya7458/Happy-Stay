public class HotelDetailsDTO
{
    public int HotelID { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string Amenities { get; set; }
    public string ImageURL { get; set; }

    public IEnumerable<RoomDTO> Rooms { get; set; }
    public IEnumerable<ReviewDTO> Reviews { get; set; }
}

public class RoomDTO
{
    public int RoomID { get; set; }
    public string RoomSize { get; set; }
    public string BedType { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal BaseFare { get; set; }
    public bool IsAC { get; set; }
    public bool AvailabilityStatus { get; set; }
}

public class ReviewDTO
{
    public int ReviewID { get; set; }
    public int UserID { get; set; }
    public int Rating { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}
