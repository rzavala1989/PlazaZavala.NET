using Microsoft.EntityFrameworkCore;
using HotelAppDataAccess.Models;
using Microsoft.Extensions.Configuration;

namespace HotelApp.DataAccess.Context
{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<HotelModel> Hotels { get; set; }
        public DbSet<RoomTypeModel> RoomTypes { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<GuestModel> Guests { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }

        private readonly IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define any additional configurations or constraints here
        }
    }
}