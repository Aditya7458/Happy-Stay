using Cozy.Models;
using Cozy.Data;
using Cozy.Repositories.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;

namespace Cozy.Repositories.Implementations
{
    public class CancellationRepository : ICancellationRepository
    {
        private readonly AppDbContext _context;

        public CancellationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cancellation> GetCancellationByIdAsync(int id)
        {
            return await _context.Cancellations.FindAsync(id);
        }

        public async Task<Cancellation> AddCancellationAsync(Cancellation cancellation)
        {
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();
            return cancellation;
        }

        public async Task<Cancellation> UpdateCancellationAsync(Cancellation cancellation)
        {
            _context.Cancellations.Update(cancellation);
            await _context.SaveChangesAsync();
            return cancellation;
        }

        public async Task<bool> DeleteCancellationAsync(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation == null) return false;

            _context.Cancellations.Remove(cancellation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
