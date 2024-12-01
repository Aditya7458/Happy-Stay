using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cozy.Models;
using Cozy.DTOs;
using Cozy.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cozy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        [HttpPost("BookRoom")]
        [Authorize]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequestDTO bookingRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if the user exists
                var user = await _userRepository.GetUserByIdAsync(bookingRequest.UserID);
                if (user == null)
                    return NotFound("User not found.");

                // Check if the room exists
                var room = await _roomRepository.GetRoomByIdAsync(bookingRequest.RoomID);
                if (room == null)
                    return NotFound("Room not found.");

                // Check room availability for the given dates
                var existingBookings = await _bookingRepository.GetBookingsByRoomIdAsync(bookingRequest.RoomID);
                bool isRoomAvailable = !existingBookings.Any(b =>
                    (bookingRequest.CheckInDate < b.CheckOutDate && bookingRequest.CheckOutDate > b.CheckInDate));

                if (!isRoomAvailable)
                    return Conflict("The room is not available for the selected dates.");

                // Calculate the total price
                var totalDays = (bookingRequest.CheckOutDate - bookingRequest.CheckInDate).Days;
                if (totalDays <= 0)
                    return BadRequest("Check-out date must be later than check-in date.");

                var totalPrice = totalDays * room.BaseFare;

                // Map DTO to Booking model
                var booking = new Booking
                {
                    UserID = bookingRequest.UserID,
                    RoomID = bookingRequest.RoomID,
                    CheckInDate = bookingRequest.CheckInDate,
                    CheckOutDate = bookingRequest.CheckOutDate,
                    NumberOfAdults = bookingRequest.NumberOfAdults,
                    NumberOfChildren = bookingRequest.NumberOfChildren,
                    TotalPrice = totalPrice,
                    Status = "Booked",
                    CreatedAt = DateTime.UtcNow
                };

                // Save the booking
                var newBooking = await _bookingRepository.AddBookingAsync(booking);

                return Ok(newBooking);
            }
            catch (Exception ex)
            {
                // Log the exception details (you can use a logging framework like Serilog or NLog)
                return StatusCode(500, new { Message = "An unexpected error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpDelete("CancelBooking/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                // Retrieve the booking
                var booking = await _bookingRepository.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound("Booking not found.");

                // Check if the booking is already completed or canceled
                if (booking.Status == "Completed")
                    return Conflict("Booking is already completed and cannot be canceled.");
                if (booking.Status == "Canceled")
                    return Conflict("Booking is already canceled.");

                // Update booking status to "Canceled"
                booking.Status = "Canceled";
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepository.UpdateBookingAsync(booking);

                return Ok(new { message = "Booking has been successfully canceled.", booking });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An unexpected error occurred while canceling the booking.", Details = ex.Message });
            }
        }

        [HttpPost("ProcessRefund/{id}")]
        [Authorize]
        public async Task<IActionResult> ProcessRefund(int id)
        {
            try
            {
                // Retrieve the booking
                var booking = await _bookingRepository.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound("Booking not found.");

                // Check if the booking is eligible for a refund
                if (booking.Status != "Canceled")
                    return Conflict("Refund can only be processed for canceled bookings.");

                // Calculate refund amount based on cancellation policy
                var daysBeforeCheckIn = (booking.CheckInDate - DateTime.UtcNow).Days;
                decimal refundAmount;

                if (daysBeforeCheckIn > 7)
                {
                    // Full refund for cancellations more than 7 days before check-in
                    refundAmount = booking.TotalPrice;
                }
                else if (daysBeforeCheckIn > 3)
                {
                    // 50% refund for cancellations between 3 and 7 days before check-in
                    refundAmount = booking.TotalPrice * 0.5m;
                }
                else
                {
                    // No refund for cancellations less than 3 days before check-in
                    refundAmount = 0m;
                }

                // Create or update a refund record (optional, depending on your implementation)
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepository.UpdateBookingAsync(booking);

                return Ok(new
                {
                    message = refundAmount > 0 ? "Refund processed successfully." : "No refund eligible for this booking.",
                    refundAmount
                });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An unexpected error occurred while processing the refund.", Details = ex.Message });
            }
        }
    }
}
