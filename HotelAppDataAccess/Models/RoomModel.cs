using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppDataAccess.Models;

public class RoomModel
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    public string RoomNumber { get; set; }

    [ForeignKey("RoomTypeId")]
    public RoomTypeModel RoomType { get; set; }
    public int RoomTypeId { get; set; }

    public List<BookingModel> Bookings { get; set; }
}