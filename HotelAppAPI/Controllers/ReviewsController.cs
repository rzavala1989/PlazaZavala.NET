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
    public class ReviewsController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(HotelContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewModel>>> GetReviews()
        {
            _logger.LogInformation("Getting all reviews.");
            var reviews = await _context.Reviews.Include(r => r.Booking).ThenInclude(b => b.Guest).ToListAsync();
            _logger.LogInformation($"Found {reviews.Count} reviews.");
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewModel>> GetReview(int id)
        {
            _logger.LogInformation($"Getting review with ID: {id}");
            var review = await _context.Reviews.Include(r => r.Booking).ThenInclude(b => b.Guest).FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null)
            {
                _logger.LogWarning($"Review with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Review with ID: {id} found.");
            return Ok(review);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewModel>> PostReview(ReviewModel review)
        {
            _logger.LogInformation($"Creating new review for booking ID: {review.BookingId}");
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, ReviewModel review)
        {
            if (id != review.ReviewId)
            {
                _logger.LogWarning($"ID mismatch: {id} does not match review ID: {review.ReviewId}");
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review with ID: {id} updated.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            _logger.LogInformation($"Deleting review with ID: {id}");
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                _logger.LogWarning($"Review with ID: {id} not found.");
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review with ID: {id} deleted.");
            return NoContent();
        }
    }
}
