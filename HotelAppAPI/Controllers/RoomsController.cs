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
    public class RoomsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(HotelContext context, ILogger<RoomsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomModel>>> GetRooms()
        {
            _logger.LogInformation("Getting all rooms.");
            var rooms = await _context.Rooms.ToListAsync();
            _logger.LogInformation($"Found {rooms.Count} rooms.");
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomModel>> GetRoom(int id)
        {
            _logger.LogInformation($"Getting room with ID: {id}");
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                _logger.LogWarning($"Room with ID: {id} not found.");
                return NotFound();
            }
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<RoomModel>> PostRoom(RoomModel room)
        {
            _logger.LogInformation($"Creating new room: {room.RoomNumber}");
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, room);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> PutRoom(int id, RoomModel room)
        {
            if (id != room.RoomId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match room ID: {room.RoomId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating room with ID: {id}");
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
