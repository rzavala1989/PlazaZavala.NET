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
    public class RoomsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public RoomsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private RoomsController CreateController(HotelContext context)
        {
            var loggerMock = new Mock<ILogger<RoomsController>>();
            return new RoomsController(context, loggerMock.Object);}

        private void ClearDatabase(HotelContext context)
        {
            context.Rooms.RemoveRange(context.Rooms);
            context.RoomTypes.RemoveRange(context.RoomTypes);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetRooms_ReturnsAllRooms()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                context.Rooms.Add(new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId });
                context.Rooms.Add(new RoomModel { RoomNumber = "102", RoomTypeId = roomType.RoomTypeId });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetRooms();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var rooms = Assert.IsType<List<RoomModel>>(okResult.Value);

                Assert.Equal(2, rooms.Count);
            }
        }

        [Fact]
        public async Task GetRoom_ReturnsRoom()
        {
            int roomId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var room = new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId };
                context.Rooms.Add(room);
                context.SaveChanges();
                roomId = room.RoomId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetRoom(roomId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var room = Assert.IsType<RoomModel>(okResult.Value);

                Assert.Equal(roomId, room.RoomId);
            }
        }

        [Fact]
        public async Task PostRoom_CreatesRoom()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var controller = CreateController(context);

                var newRoom = new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId };

                var result = await controller.PostRoom(newRoom);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var room = Assert.IsType<RoomModel>(createdAtActionResult.Value);

                Assert.Equal(newRoom.RoomNumber, room.RoomNumber);
                Assert.Equal(newRoom.RoomTypeId, room.RoomTypeId);
            }
        }

        [Fact]
        public async Task PutRoom_UpdatesRoom()
        {
            int roomId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var room = new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId };
                context.Rooms.Add(room);
                context.SaveChanges();
                roomId = room.RoomId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedRoom = new RoomModel { RoomId = roomId, RoomNumber = "102", RoomTypeId = context.RoomTypes.First().RoomTypeId };

                var result = await controller.PutRoom(roomId, updatedRoom);

                Assert.IsType<NoContentResult>(result);

                var room = await context.Rooms.FindAsync(roomId);
                Assert.Equal(updatedRoom.RoomNumber, room.RoomNumber);
            }
        }

        [Fact]
        public async Task DeleteRoom_DeletesRoom()
        {
            int roomId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();

                var room = new RoomModel { RoomNumber = "101", RoomTypeId = roomType.RoomTypeId };
                context.Rooms.Add(room);
                context.SaveChanges();
                roomId = room.RoomId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteRoom(roomId);

                Assert.IsType<NoContentResult>(result);

                var room = await context.Rooms.FindAsync(roomId);
                Assert.Null(room);
            }
        }
    }
}
