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
    public class GuestsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public GuestsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private GuestsController CreateController(HotelContext context)
        {
            var loggerMock = new Mock<ILogger<GuestsController>>();
            return new GuestsController(context, loggerMock.Object);
        }

        [Fact]
        public async Task GetGuests_ReturnsAllGuests()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                context.Guests.Add(new GuestModel
                { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" });

                context.Guests.Add(new GuestModel
                { FirstName = "Jane", LastName = "Doe", Phone = "987-654-3210", Email = "jane.doe@example.com" });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetGuests();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var guests = Assert.IsType<List<GuestModel>>(okResult.Value);

                Assert.Equal(2, guests.Count);
            }
        }

        [Fact]
        public async Task GetGuest_ReturnsGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var guest = new GuestModel
                { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetGuest(guestId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var guest = Assert.IsType<GuestModel>(okResult.Value);

                Assert.Equal("John", guest.FirstName);
                Assert.Equal("Doe", guest.LastName);
            }
        }

        [Fact]
        public async Task PostGuest_CreatesGuest()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var newGuest = new GuestModel
                { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };

                var result = await controller.PostGuest(newGuest);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var guest = Assert.IsType<GuestModel>(createdAtActionResult.Value);

                Assert.Equal("John", guest.FirstName);
                Assert.Equal("Doe", guest.LastName);
            }
        }

        [Fact]
        public async Task PutGuest_UpdatesGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var guest = new GuestModel
                { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedGuest = new GuestModel
                {
                    GuestModelId = guestId, FirstName = "Johnny", LastName = "Doe", Phone = "123-456-7890",
                    Email = "john.doe@example.com"
                };

                var result = await controller.PutGuest(guestId, updatedGuest);

                Assert.IsType<NoContentResult>(result);

                var guest = await context.Guests.FindAsync(guestId);
                Assert.Equal("Johnny", guest.FirstName);
            }
        }

        [Fact]
        public async Task DeleteGuest_DeletesGuest()
        {
            int guestId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var guest = new GuestModel
                { FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Email = "john.doe@example.com" };
                context.Guests.Add(guest);
                context.SaveChanges();
                guestId = guest.GuestModelId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteGuest(guestId);

                Assert.IsType<NoContentResult>(result);

                var guest = await context.Guests.FindAsync(guestId);
                Assert.Null(guest);
            }
        }
    }
}
