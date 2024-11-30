public class SearchHotelDTO
{
    public string Location { get; set; }
    public string Name { get; set; }
    public int NumberOfRooms { get; set; } = 1;
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
}
