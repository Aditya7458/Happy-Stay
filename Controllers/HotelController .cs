using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cozy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        #region Admin-Only Operations
        // POST: api/hotel/add
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]  // Only Admin can add hotels
        public async Task<IActionResult> AddHotel([FromBody] AddHotelDTO hotelDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Map the DTO to the Hotel model
                var hotel = new Hotel
                {
                    Name = hotelDTO.Name,
                    Location = hotelDTO.Location,
                    Description = hotelDTO.Description,
                    Amenities = hotelDTO.Amenities,
                    ImageURL = hotelDTO.ImageURL,
                    IsActive = hotelDTO.IsActive,
                    CreatedBy = hotelDTO.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdHotel = await _hotelRepository.AddHotelAsync(hotel);
                return Ok(new { Message = "Hotel added successfully", Hotel = createdHotel });
            }
            catch (Exception ex)
            {
                // Log the exception details (you can use a logging framework like Serilog or NLog)
                return StatusCode(500, new { Message = "An error occurred while adding the hotel.", Details = ex.Message });
            }
        }

        // PUT: api/hotel/update
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]  // Only Admin can update hotels
        public async Task<IActionResult> UpdateHotel([FromBody] UpdateHotelDTO hotelDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var hotel = await _hotelRepository.GetHotelByIdAsync(hotelDTO.HotelID);
                if (hotel == null)
                    return NotFound("Hotel not found.");

                // Update hotel properties
                hotel.Name = hotelDTO.Name;
                hotel.Location = hotelDTO.Location;
                hotel.Description = hotelDTO.Description;
                hotel.Amenities = hotelDTO.Amenities;
                hotel.ImageURL = hotelDTO.ImageURL;
                hotel.IsActive = hotelDTO.IsActive;
                hotel.UpdatedAt = DateTime.UtcNow;

                var updatedHotel = await _hotelRepository.UpdateHotelAsync(hotel);
                return Ok(new { Message = "Hotel updated successfully", Hotel = updatedHotel });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An error occurred while updating the hotel.", Details = ex.Message });
            }
        }

        // DELETE: api/hotel/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin can delete hotels
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                var success = await _hotelRepository.DeleteHotelAsync(id);
                if (!success)
                    return NotFound("Hotel not found.");

                return Ok(new { Message = "Hotel deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An error occurred while deleting the hotel.", Details = ex.Message });
            }
        }
        #endregion

        #region User Operations
        // GET: api/hotel/Search
        [HttpGet("Search")]
        [Authorize]  // Any authenticated user can search hotels
        public async Task<IActionResult> SearchHotels([FromQuery] SearchHotelDTO searchDTO)
        {
            try
            {
                var hotels = await _hotelRepository.GetAllHotelsAsync(); // Get all hotels

                // Filter by location if provided (case-insensitive)
                if (!string.IsNullOrEmpty(searchDTO.Location))
                {
                    hotels = hotels.Where(h => h.Location.Contains(searchDTO.Location, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Filter by name if provided (case-insensitive)
                if (!string.IsNullOrEmpty(searchDTO.Name))
                {
                    hotels = hotels.Where(h => h.Name.Contains(searchDTO.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Additional filters based on rooms and occupancy
                hotels = hotels.Where(hotel =>
                    hotel.Rooms.Any(room =>
                        room.AvailabilityStatus &&
                        room.MaxOccupancy >= (searchDTO.Adults + searchDTO.Children) &&
                        room.HotelID == hotel.HotelID)  // Ensure rooms match the occupancy conditions
                ).ToList();

                // Limit the number of rooms based on the requested NumberOfRooms
                var result = hotels.Select(hotel => new
                {
                    hotel.HotelID,
                    hotel.Name,
                    hotel.Location,
                    hotel.Description,
                    hotel.Amenities,
                    hotel.ImageURL,
                    Rooms = hotel.Rooms.Where(room =>
                        room.AvailabilityStatus &&
                        room.MaxOccupancy >= (searchDTO.Adults + searchDTO.Children) &&
                        room.HotelID == hotel.HotelID)
                        .Take(searchDTO.NumberOfRooms) // Limit the number of rooms returned
                        .Select(room => new
                        {
                            room.RoomID,
                            room.RoomSize,
                            room.BedType,
                            room.MaxOccupancy,
                            room.BaseFare,
                            room.IsAC
                        })
                        .ToList()
                }).Where(h => h.Rooms.Any()); // Only include hotels with available rooms

                // Return the filtered list of hotels with their available rooms
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { message = "An error occurred while searching for hotels.", error = ex.Message });
            }
        }

        // GET: api/hotel/{id}
        [HttpGet("{id}")]
        [Authorize]  // Any authenticated user can view hotel details
        public async Task<IActionResult> GetHotelDetails(int id)
        {
            try
            {
                var hotel = await _hotelRepository.GetHotelByIdAsync(id);

                if (hotel == null)
                    return NotFound(new { message = "Hotel not found." });

                var result = new HotelDetailsDTO
                {
                    HotelID = hotel.HotelID,
                    Name = hotel.Name,
                    Location = hotel.Location,
                    Description = hotel.Description,
                    Amenities = hotel.Amenities,
                    ImageURL = hotel.ImageURL,
                    Rooms = hotel.Rooms.Select(room => new RoomDTO
                    {
                        RoomID = room.RoomID,
                        RoomSize = room.RoomSize,
                        BedType = room.BedType,
                        MaxOccupancy = room.MaxOccupancy,
                        BaseFare = room.BaseFare,
                        IsAC = room.IsAC,
                        AvailabilityStatus = room.AvailabilityStatus
                    }),
                    Reviews = hotel.Reviews.Select(review => new ReviewDTO
                    {
                        ReviewID = review.ReviewID,
                        UserID = review.UserID,
                        Rating = review.Rating,
                        ReviewText = review.ReviewText,
                        CreatedAt = review.CreatedAt
                    })
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { message = "An error occurred while fetching hotel details.", error = ex.Message });
            }
        }
        #endregion
    }
}
