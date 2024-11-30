using Cozy.Models;
using Cozy.Data;
using Cozy.Repositories.Interfaces;
using System;
using Microsoft.EntityFrameworkCore;


namespace Cozy.Repositories.Implementations
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Hotel> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)   // Include related rooms
                .Include(h => h.Reviews) // Include related reviews
                .FirstOrDefaultAsync(h => h.HotelID == id);
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .ToListAsync();
        }

        public async Task<Hotel> AddHotelAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
