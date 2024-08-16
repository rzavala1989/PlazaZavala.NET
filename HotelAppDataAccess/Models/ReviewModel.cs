using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models;

public class ReviewModel
{
    [Key]
    public int ReviewId { get; set; }

    public string Content { get; set; }
    public int Rating { get; set; }

    [ForeignKey("BookingId")]
    public BookingModel Booking { get; set; }
    public int BookingId { get; set; }

    // Optional: Direct link to Hotel or Room
    [ForeignKey("HotelId")]
    public HotelModel Hotel { get; set; }
    public int? HotelId { get; set; } // Nullable if you want reviews to be either for bookings or hotels directly
}