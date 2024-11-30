//using Cozy.Models;
//using Cozy.Repositories.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Cozy.Controllers
//{
//    [ApiController]
//    [Route("api/Hotels")]
//    [Authorize]
//    public class HotelController : ControllerBase
//    {
//        private readonly IHotelRepository _hotelRepository;

//        public HotelController(IHotelRepository hotelRepository)
//        {
//            _hotelRepository = hotelRepository;
//        }

//        // GET: api/Hotels/Search
//        [HttpGet("Search")]
//        public async Task<IActionResult> SearchHotels(
//            [FromQuery] string location,
//            [FromQuery] DateTime checkInDate,
//            [FromQuery] DateTime checkOutDate,
//            [FromQuery] int numberOfRooms = 1,
//            [FromQuery] int adults = 1,
//            [FromQuery] int children = 0)
//        {
//            try
//            {
//                var hotels = await _hotelRepository.GetAllHotelsAsync();

//                // Filter by location
//                hotels = hotels.Where(h => h.Location!.Contains(location, StringComparison.OrdinalIgnoreCase));

//                // Map hotel information and availability
//                var result = hotels.Select(hotel => new
//                {
//                    hotel.HotelID,
//                    hotel.Name,
//                    hotel.Location,
//                    hotel.Description,
//                    hotel.Amenities,
//                    hotel.ImageURL,
//                    Rooms = hotel.Rooms.Where(room =>
//                        room.AvailabilityStatus &&
//                        room.MaxOccupancy >= (adults + children) &&
//                        room.HotelID == hotel.HotelID)
//                        .Select(room => new
//                        {
//                            room.RoomID,
//                            room.RoomSize,
//                            room.BedType,
//                            room.MaxOccupancy,
//                            room.BaseFare,
//                            room.IsAC
//                        })
//                        .ToList()
//                }).Where(h => h.Rooms.Any()); // Only include hotels with available rooms

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred while searching for hotels.", error = ex.Message });
//            }
//        }

//        // GET: api/Hotels/{id}
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetHotelDetails(int id)
//        {
//            try
//            {
//                var hotel = await _hotelRepository.GetHotelByIdAsync(id);

//                if (hotel == null)
//                    return NotFound(new { message = "Hotel not found." });

//                var result = new
//                {
//                    hotel.HotelID,
//                    hotel.Name,
//                    hotel.Location,
//                    hotel.Description,
//                    hotel.Amenities,
//                    hotel.ImageURL,
//                    Rooms = hotel.Rooms.Select(room => new
//                    {
//                        room.RoomID,
//                        room.RoomSize,
//                        room.BedType,
//                        room.MaxOccupancy,
//                        room.BaseFare,
//                        room.IsAC,
//                        room.AvailabilityStatus
//                    }),
//                    Reviews = hotel.Reviews.Select(review => new
//                    {
//                        review.ReviewID,
//                        review.UserID,
//                        review.Rating,
//                        review.ReviewText,
//                        review.CreatedAt
//                    })
//                };

//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred while fetching hotel details.", error = ex.Message });
//            }
//        }
//    }
//}
