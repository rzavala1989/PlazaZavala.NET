using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models
{
    public class BookingModel
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
        
        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        [ForeignKey("Guest")]
        public int GuestId { get; set; }
        
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime EndDate { get; set; }
        
        public decimal TotalCost { get; set; }

        public RoomTypeModel RoomType { get; set; }
        public GuestModel Guest { get; set; }
    }
}