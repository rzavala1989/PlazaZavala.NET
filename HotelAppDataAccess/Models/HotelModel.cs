using System.ComponentModel.DataAnnotations;

namespace HotelAppDataAccess.Models;

public class HotelModel
{
    [Key]
    public int HotelId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}