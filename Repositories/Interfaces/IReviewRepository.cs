using Cozy.Models;

namespace Cozy.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByHotelIdAsync(int hotelId);
        Task<Review> AddReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
    }
}
