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
    public class RoomTypesControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public RoomTypesControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private RoomTypesController CreateController(HotelContext context)
        {
            var loggerMock = new Mock<ILogger<RoomTypesController>>();
            return new RoomTypesController(context, loggerMock.Object);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.RoomTypes.RemoveRange(context.RoomTypes);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetRoomTypes_ReturnsAllRoomTypes()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                context.RoomTypes.Add(new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 });
                context.RoomTypes.Add(new RoomTypeModel { TypeName = "Deluxe", Description = "Deluxe Room", Price = 200 });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetRoomTypes();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var roomTypes = Assert.IsType<List<RoomTypeModel>>(okResult.Value);

                Assert.Equal(2, roomTypes.Count);
            }
        }

        [Fact]
        public async Task GetRoomType_ReturnsRoomType()
        {
            int roomTypeId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();
                roomTypeId = roomType.RoomTypeId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetRoomType(roomTypeId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var roomType = Assert.IsType<RoomTypeModel>(okResult.Value);

                Assert.Equal(roomTypeId, roomType.RoomTypeId);
            }
        }

        [Fact]
        public async Task PostRoomType_CreatesRoomType()
        {
            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var controller = CreateController(context);

                var newRoomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };

                var result = await controller.PostRoomType(newRoomType);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var roomType = Assert.IsType<RoomTypeModel>(createdAtActionResult.Value);

                Assert.Equal(newRoomType.TypeName, roomType.TypeName);
                Assert.Equal(newRoomType.Description, roomType.Description);
                Assert.Equal(newRoomType.Price, roomType.Price);
            }
        }

        [Fact]
        public async Task PutRoomType_UpdatesRoomType()
        {
            int roomTypeId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();
                roomTypeId = roomType.RoomTypeId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedRoomType = new RoomTypeModel { RoomTypeId = roomTypeId, TypeName = "Deluxe", Description = "Deluxe Room", Price = 200 };

                var result = await controller.PutRoomType(roomTypeId, updatedRoomType);

                Assert.IsType<NoContentResult>(result);

                var roomType = await context.RoomTypes.FindAsync(roomTypeId);
                Assert.Equal(updatedRoomType.TypeName, roomType.TypeName);
                Assert.Equal(updatedRoomType.Description, roomType.Description);
                Assert.Equal(updatedRoomType.Price, roomType.Price);
            }
        }

        [Fact]
        public async Task DeleteRoomType_DeletesRoomType()
        {
            int roomTypeId;

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                ClearDatabase(context);

                var roomType = new RoomTypeModel { TypeName = "Standard", Description = "Standard Room", Price = 100 };
                context.RoomTypes.Add(roomType);
                context.SaveChanges();
                roomTypeId = roomType.RoomTypeId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteRoomType(roomTypeId);

                Assert.IsType<NoContentResult>(result);

                var roomType = await context.RoomTypes.FindAsync(roomTypeId);
                Assert.Null(roomType);
            }
        }
    }
}
