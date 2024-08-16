using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<GuestsController> _logger;

        public GuestsController(HotelContext context, ILogger<GuestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestModel>>> GetGuests()
        {
            _logger.LogInformation("Getting all guests.");
            var guests = await _context.Guests.ToListAsync();
            _logger.LogInformation($"Found {guests.Count} guests.");
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GuestModel>> GetGuest(int id)
        {
            _logger.LogInformation($"Getting guest with ID: {id}");
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                _logger.LogWarning($"Guest with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Guest with ID: {id} found.");
            return Ok(guest);
        }

        [HttpPost]
        public async Task<ActionResult<GuestModel>> PostGuest(GuestModel guest)
        {
            _logger.LogInformation($"Creating new guest: {guest.FirstName} {guest.LastName}");
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGuest), new { id = guest.GuestModelId }, guest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuest(int id, GuestModel guest)
        {
            if (id != guest.GuestModelId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match guest ID: {guest.GuestModelId}");
                return BadRequest();
            }

            _context.Entry(guest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Guest with ID: {id} updated.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            _logger.LogInformation($"Deleting guest with ID: {id}");
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                _logger.LogWarning($"Guest with ID: {id} not found.");
                return NotFound();
            }

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Guest with ID: {id} deleted.");
            return NoContent();
        }
    }
}
