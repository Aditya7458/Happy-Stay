using Cozy.Models;

namespace Cozy.Repositories.Interfaces
{
    public interface IHotelRepository
    {
        Task<Hotel> GetHotelByIdAsync(int id);
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel> AddHotelAsync(Hotel hotel);
        Task<Hotel> UpdateHotelAsync(Hotel hotel);
        Task<bool> DeleteHotelAsync(int id);
    }
}
