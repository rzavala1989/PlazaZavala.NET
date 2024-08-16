using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.DataAccess.Context;
using HotelAppAPI.Services;
using HotelAppDataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<BookingsController> _logger;
        private readonly BookingService _bookingService;
        private readonly EmailService _emailService;

        public BookingsController(HotelContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingModel>>> GetBookings()
        {
            _logger.LogInformation("Getting all bookings.");
            var bookings = await _context.Bookings.Include(b => b.Guest).Include(b => b.RoomType).ToListAsync();
            _logger.LogInformation($"Found {bookings.Count} bookings.");
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingModel>> GetBooking(int id)
        {
            _logger.LogInformation($"Getting booking with ID: {id}");
            var booking = await _context.Bookings.Include(b => b.Guest).Include(b => b.RoomType).FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Booking with ID: {id} found.");
            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult<BookingModel>> PostBooking(BookingModel booking)
        {
            var overlappingBooking = await _context.Bookings
                .AnyAsync(b => b.GuestId == booking.GuestId && 
                               b.HotelId != booking.HotelId && 
                               ((booking.StartDate >= b.StartDate && booking.StartDate <= b.EndDate) ||
                                (booking.EndDate >= b.StartDate && booking.EndDate <= b.EndDate)));

            if (overlappingBooking)
            {
                return BadRequest("Cannot book at multiple hotels with the same credentials at the same time.");
            }

            var createdBooking = await _bookingService.CreateBooking(booking);
            if (createdBooking == null)
            {
                return BadRequest("Room is not available for the selected dates.");
            }

            _logger.LogInformation($"Creating new booking for guest ID: {booking.GuestId}");

            // Send booking confirmation email
            var guest = await _context.Guests.FindAsync(booking.GuestId);
            if (guest != null)
            {
                _emailService.SendBookingConfirmationEmail(guest.Email, "Booking Confirmation", "Your booking is confirmed.");
            }

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, BookingModel booking)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            var timeSinceLastUpdate = DateTime.UtcNow - existingBooking.StartDate; // Assuming LastUpdated is a field tracking the last update time
            if (timeSinceLastUpdate.TotalHours < 24)
            {
                return BadRequest("Booking cannot be changed within 24 hours of the last update.");
            }
            _logger.LogInformation($"Updating booking with ID: {id}");
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            _logger.LogInformation($"Deleting booking with ID: {id}");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID: {id} not found.");
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Booking with ID: {id} deleted.");
            return NoContent();
        }
        
        // Search bookings by date range
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookingModel>>> SearchBookings([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var bookings = await _context.Bookings
                .Where(b => b.StartDate >= startDate && b.EndDate <= endDate)
                .ToListAsync();

            return Ok(bookings);
        }
        
        // Search bookings by guest ID
        [HttpGet("search/guest")]
        public async Task<ActionResult<IEnumerable<BookingModel>>> SearchBookingsByGuestId([FromQuery] int guestId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.GuestId == guestId)
                .ToListAsync();

            return Ok(bookings);
        }
        
        // Get bookings by hotel ID
        [HttpGet("hotel")]
        public async Task<ActionResult<IEnumerable<BookingModel>>> GetBookingsByHotelId([FromQuery] int hotelId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.HotelId == hotelId)
                .ToListAsync();

            return Ok(bookings);
        }
        
        // Generate and return reports as needed
        [HttpGet("reports/occupancy")]
        public async Task<ActionResult<OccupancyReportModel>> GetOccupancyReport()
        {
            var occupancyRate = await _context.Bookings
                .GroupBy(b => b.RoomId)
                .Select(g => new { RoomId = g.Key, OccupancyRate = g.Count() / 365.0 }) // Example calculation
                .ToListAsync();

            return Ok(occupancyRate);
        }

        
        
        
        
        

    }
}
