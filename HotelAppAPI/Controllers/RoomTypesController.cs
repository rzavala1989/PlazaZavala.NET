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
    public class RoomTypesController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<RoomTypesController> _logger;

        public RoomTypesController(HotelContext context, ILogger<RoomTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeModel>>> GetRoomTypes()
        {
            _logger.LogInformation("Getting all room types.");
            var roomTypes = await _context.RoomTypes.ToListAsync();
            _logger.LogInformation($"Found {roomTypes.Count} room types.");
            return Ok(roomTypes);  // Ensure to return OkObjectResult
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeModel>> GetRoomType(int id)
        {
            _logger.LogInformation($"Getting room type with ID: {id}");
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                _logger.LogWarning($"Room type with ID: {id} not found.");
                return NotFound();
            }
            return Ok(roomType);  // Ensure to return OkObjectResult
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<RoomTypeModel>> PostRoomType(RoomTypeModel roomType)
        {
            _logger.LogInformation($"Creating new room type: {roomType.TypeName}");
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoomType), new { id = roomType.RoomTypeId }, roomType);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> PutRoomType(int id, RoomTypeModel roomType)
        {
            if (id != roomType.RoomTypeId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match room type ID: {roomType.RoomTypeId}");
                return BadRequest();
            }
            _logger.LogInformation($"Updating room type with ID: {id}");
            _context.Entry(roomType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteRoomType(int id)
        {
            _logger.LogInformation($"Deleting room type with ID: {id}");
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                _logger.LogWarning($"Room type with ID: {id} not found.");
                return NotFound();
            }
            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
