using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cozy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        #region Admin Operations

        // POST: api/room/add
        [HttpPost("add")]
        [Authorize(Roles = "Admin")] // Only Admin can add rooms
        public async Task<IActionResult> AddRoom([FromBody] AddRoomDTO roomDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = new Room
                {
                    HotelID = roomDTO.HotelID,
                    RoomSize = roomDTO.RoomSize,
                    BedType = roomDTO.BedType,
                    MaxOccupancy = roomDTO.MaxOccupancy,
                    BaseFare = roomDTO.BaseFare,
                    IsAC = roomDTO.IsAC,
                    AvailabilityStatus = roomDTO.AvailabilityStatus,
                    CreatedBy = roomDTO.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdRoom = await _roomRepository.AddRoomAsync(room);
                return Ok(new { Message = "Room added successfully", Room = createdRoom });
            }
            catch (Exception ex)
            {
                // Log the exception details (you can use a logging framework like Serilog or NLog)
                return StatusCode(500, new { Message = "An error occurred while adding the room.", Details = ex.Message });
            }
        }

        // PUT: api/room/update
        [HttpPut("update")]
        [Authorize(Roles = "Admin")] // Only Admin can update rooms
        public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomDTO roomDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = await _roomRepository.GetRoomByIdAsync(roomDTO.RoomID);
                if (room == null)
                    return NotFound(new { Message = "Room not found" });

                // Update room properties
                room.RoomSize = roomDTO.RoomSize;
                room.BedType = roomDTO.BedType;
                room.MaxOccupancy = roomDTO.MaxOccupancy;
                room.BaseFare = roomDTO.BaseFare;
                room.IsAC = roomDTO.IsAC;
                room.AvailabilityStatus = roomDTO.AvailabilityStatus;
                room.UpdatedAt = DateTime.UtcNow;

                var updatedRoom = await _roomRepository.UpdateRoomAsync(room);
                return Ok(new { Message = "Room updated successfully", Room = updatedRoom });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An error occurred while updating the room.", Details = ex.Message });
            }
        }

        // DELETE: api/room/delete/{id}
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete rooms
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var success = await _roomRepository.DeleteRoomAsync(id);
                if (!success)
                    return NotFound(new { Message = "Room not found" });

                return Ok(new { Message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception details
                return StatusCode(500, new { Message = "An error occurred while deleting the room.", Details = ex.Message });
            }
        }

        #endregion
    }
}
