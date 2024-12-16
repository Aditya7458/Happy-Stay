namespace Cozy.DTOs
{
    public class BookingResponseDTO
    {
        public int BookingID { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? HotelName { get; set; }
        public string? HotelLocation { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public string? Status { get; set; }
    }
}
