using Cozy.Models;
using Cozy.Data;
using Cozy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cozy.DTOs;

namespace Cozy.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserID == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId) // New method
        {
            return await _context.Bookings
                .Where(b => b.RoomID == roomId && b.Status == "Booked") // Only active bookings
                .ToListAsync();
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<BookingResponseDTO>> GetAllBookingsWithDetailsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User) // Include user details
                .Include(b => b.Room) // Include room to access hotel
                    .ThenInclude(r => r.Hotel)
                .Select(b => new BookingResponseDTO
                {
                    BookingID = b.BookingID,
                    UserName = b.User.Username,
                    UserEmail = b.User.Email,
                    HotelName = b.Room.Hotel.Name,
                    HotelLocation = b.Room.Hotel.Location,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    NumberOfAdults = b.NumberOfAdults,
                    NumberOfChildren = b.NumberOfChildren,
                    Status = b.Status
                })
                .ToListAsync();
        }
    }
}
