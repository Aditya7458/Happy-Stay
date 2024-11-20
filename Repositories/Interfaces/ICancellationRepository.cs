using Cozy.Models;

namespace Cozy.Repositories.Interfaces
{
    public interface ICancellationRepository
    {
        Task<Cancellation> GetCancellationByIdAsync(int id);
        Task<Cancellation> AddCancellationAsync(Cancellation cancellation);
        Task<Cancellation> UpdateCancellationAsync(Cancellation cancellation);
        Task<bool> DeleteCancellationAsync(int id);
    }
}
