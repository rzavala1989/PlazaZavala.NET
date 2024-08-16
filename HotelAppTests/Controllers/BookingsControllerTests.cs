using HotelApp.DataAccess.Context;
using HotelAppAPI.Controllers;
using HotelAppDataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelAppTests.Controllers
{
    public class BookingsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public BookingsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private BookingsController CreateController(HotelContext context)
        {
            var loggerMock = new Mock<ILogger<BookingsController>>();
            return new BookingsController(context, loggerMock.Object);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.Bookings.RemoveRange(context.Bookings);
            context.Guests.RemoveRange(context.Guests);
            context.RoomTypes.RemoveRange(context.RoomTypes);
            context.Rooms.RemoveRange(context.Rooms);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetBookings_ReturnsAllBookings()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();

                context.Bookings.Add(new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetBookings();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var bookings = Assert.IsType<List<BookingModel>>(okResult.Value);

                Assert.Single(bookings); // Ensures there is exactly one booking
            }
        }

        [Fact]
        public async Task GetBooking_ReturnsBooking()
        {
            int bookingId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();

                var booking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
                context.Bookings.Add(booking);
                context.SaveChanges();
                bookingId = booking.BookingId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetBooking(bookingId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var booking = Assert.IsType<BookingModel>(okResult.Value);

                Assert.Equal(bookingId, booking.BookingId);
            }
        }

        [Fact]
        public async Task PostBooking_CreatesBooking()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();

                var controller = CreateController(context);

                var newBooking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

                var result = await controller.PostBooking(newBooking);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var booking = Assert.IsType<BookingModel>(createdAtActionResult.Value);

                Assert.Equal(newBooking.RoomTypeId, booking.RoomTypeId);
                Assert.Equal(newBooking.GuestId, booking.GuestId);
            }
        }

        [Fact]
        public async Task PutBooking_UpdatesBooking()
        {
            int bookingId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();

                var booking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
                context.Bookings.Add(booking);
                context.SaveChanges();
                bookingId = booking.BookingId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedBooking = new BookingModel { BookingId = bookingId, RoomTypeId = context.RoomTypes.First().RoomTypeId, GuestId = context.Guests.First().GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(2) };

                var result = await controller.PutBooking(bookingId, updatedBooking);

                Assert.IsType<NoContentResult>(result);

                var booking = await context.Bookings.FindAsync(bookingId);
                Assert.Equal(updatedBooking.EndDate, booking.EndDate);
            }
        }

        [Fact]
        public async Task DeleteBooking_DeletesBooking()
        {
            int bookingId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var guest = new GuestModel { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();

                var booking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
                context.Bookings.Add(booking);
                context.SaveChanges();
                bookingId = booking.BookingId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteBooking(bookingId);

                Assert.IsType<NoContentResult>(result);

                var booking = await context.Bookings.FindAsync(bookingId);
                Assert.Null(booking);
            }
        }
    }
}
