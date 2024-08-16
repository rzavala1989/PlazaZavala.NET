using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using Microsoft.EntityFrameworkCore;

public class BookingService
{
    private readonly HotelContext _context;

    public BookingService(HotelContext context)
    {
        _context = context;
    }

    public async Task<bool> IsRoomAvailable(int roomId, DateTime startDate, DateTime endDate)
    {
        return !await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.StartDate < endDate &&
            b.EndDate > startDate);
    }

    public async Task<bool> IsGuestBookingValid(int guestId, DateTime startDate, DateTime endDate)
    {
        return !await _context.Bookings.AnyAsync(b =>
            b.GuestId == guestId &&
            ((startDate >= b.StartDate && startDate <= b.EndDate) ||
             (endDate >= b.StartDate && endDate <= b.EndDate)));
    }

    public async Task<BookingModel> CreateBooking(BookingModel booking)
    {
        if (await IsRoomAvailable(booking.RoomId, booking.StartDate, booking.EndDate) &&
            await IsGuestBookingValid(booking.GuestId, booking.StartDate, booking.EndDate))
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        return null;
    }
}