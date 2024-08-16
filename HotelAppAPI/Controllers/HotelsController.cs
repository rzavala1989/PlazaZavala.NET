using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(HotelContext context, ILogger<HotelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelModel>>> GetHotels()
        {
            _logger.LogInformation("Getting all hotels.");
            var hotels = await _context.Hotels.ToListAsync();
            _logger.LogInformation($"Found {hotels.Count} hotels.");
            return Ok(hotels);  // Ensure to return OkObjectResult
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelModel>> GetHotel(int id)
        {
            _logger.LogInformation($"Getting hotel with ID: {id}");
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning($"Hotel with ID: {id} not found.");
                return NotFound();
            }
            return Ok(hotel);  // Ensure to return OkObjectResult
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HotelModel>> PostHotel(HotelModel hotel)
        {
            _logger.LogInformation($"Creating new hotel: {hotel.Name}");
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelId }, hotel);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> PutHotel(int id, HotelModel hotel)
        {
            if (id != hotel.HotelId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match hotel ID: {hotel.HotelId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating hotel with ID: {id}");
            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation($"Deleting hotel with ID: {id}");
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning($"Hotel with ID: {id} not found.");
                return NotFound();
            }
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
