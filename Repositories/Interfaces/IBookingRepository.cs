﻿using Cozy.DTOs;
using Cozy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cozy.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId); // New method
        Task<Booking> AddBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

        Task<IEnumerable<BookingResponseDTO>> GetAllBookingsWithDetailsAsync();
    }
}
