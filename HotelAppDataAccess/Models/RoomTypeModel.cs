using System.ComponentModel.DataAnnotations;

namespace HotelAppDataAccess.Models;

public class RoomTypeModel
{
    [Key]
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}