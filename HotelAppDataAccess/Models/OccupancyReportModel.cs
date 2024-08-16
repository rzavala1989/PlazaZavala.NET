namespace HotelAppDataAccess.Models;

public class OccupancyReportModel
{
    
        public int RoomId { get; set; }
        public double OccupancyRate { get; set; }
        public int TotalBookings { get; set; }
        public string RoomType { get; set; }
    
}