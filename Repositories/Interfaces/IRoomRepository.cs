using Cozy.Models;

namespace Cozy.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId);
        Task<Room> AddRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
    }
}
