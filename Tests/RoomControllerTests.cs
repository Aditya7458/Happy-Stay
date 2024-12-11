using Cozy.Controllers;
using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cozy.Tests.Controllers
{
    public class RoomControllerTests
    {
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly RoomController _controller;

        public RoomControllerTests()
        {
            _mockRoomRepo = new Mock<IRoomRepository>();
            _controller = new RoomController(_mockRoomRepo.Object);
        }

        [Fact]
        public async Task AddRoom_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidRoomDTO = new AddRoomDTO(); // Invalid data (or empty)
            _controller.ModelState.AddModelError("HotelID", "Required");

            // Act
            var result = await _controller.AddRoom(invalidRoomDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddRoom_ReturnsOk_WhenRoomIsAddedSuccessfully()
        {
            // Arrange
            var validRoomDTO = new AddRoomDTO { HotelID = 1, RoomSize = "30", BedType = "King", MaxOccupancy = 2, BaseFare = 100, IsAC = true, AvailabilityStatus = true, CreatedBy = 1 };
            var room = new Room { RoomID = 1, HotelID = 1, RoomSize = "30", BedType = "King", MaxOccupancy = 2, BaseFare = 100, IsAC = true, AvailabilityStatus = true, CreatedBy = 1 };
            _mockRoomRepo.Setup(repo => repo.AddRoomAsync(It.IsAny<Room>())).ReturnsAsync(room);

            // Act
            var result = await _controller.AddRoom(validRoomDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.Equal("Room added successfully", response.Message);
        }

        [Fact]
        public async Task UpdateRoom_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            var updateRoomDTO = new UpdateRoomDTO { RoomID = 999, RoomSize = "35", BedType = "Queen", MaxOccupancy = 3 };
            _mockRoomRepo.Setup(repo => repo.GetRoomByIdAsync(999)).ReturnsAsync((Room)null);

            // Act
            var result = await _controller.UpdateRoom(updateRoomDTO);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = notFoundResult.Value as dynamic;
            Assert.Equal("Room not found", response.Message);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsOk_WhenRoomIsDeleted()
        {
            // Arrange
            _mockRoomRepo.Setup(repo => repo.DeleteRoomAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRoom(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.Equal("Room deleted successfully", response.Message);
        }
    }
}
