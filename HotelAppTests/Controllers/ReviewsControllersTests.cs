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
    public class ReviewsControllerTests
    {
        private readonly DbContextOptions<HotelContext> _dbContextOptions;

        public ReviewsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelContext>()
                .UseInMemoryDatabase(databaseName: "HotelAppTest")
                .Options;
        }

        private ReviewsController CreateController(HotelContext context)
        {
            var loggerMock = new Mock<ILogger<ReviewsController>>();
            return new ReviewsController(context, loggerMock.Object);
        }

        private void ClearDatabase(HotelContext context)
        {
            context.Reviews.RemoveRange(context.Reviews);
            context.Bookings.RemoveRange(context.Bookings);
            context.Guests.RemoveRange(context.Guests);
            context.RoomTypes.RemoveRange(context.RoomTypes);
            context.Rooms.RemoveRange(context.Rooms);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetReviews_ReturnsAllReviews()
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

                var booking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
                context.Bookings.Add(booking);
                context.SaveChanges();

                context.Reviews.Add(new ReviewModel { BookingId = booking.BookingId, Rating = 5, Content = "Great stay!" });
                context.SaveChanges();
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetReviews();

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var reviews = Assert.IsType<List<ReviewModel>>(okResult.Value);

                Assert.Single(reviews); // Ensures there is exactly one review
            }
        }

        [Fact]
        public async Task GetReview_ReturnsReview()
        {
            int reviewId;

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

                var review = new ReviewModel { BookingId = booking.BookingId, Rating = 5, Content = "Great stay!" };
                context.Reviews.Add(review);
                context.SaveChanges();
                reviewId = review.ReviewId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.GetReview(reviewId);

                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var review = Assert.IsType<ReviewModel>(okResult.Value);

                Assert.Equal(reviewId, review.ReviewId);
            }
        }

        [Fact]
        public async Task PostReview_CreatesReview()
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

                var booking = new BookingModel { RoomTypeId = roomType.RoomTypeId, GuestId = guest.GuestModelId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
                context.Bookings.Add(booking);
                context.SaveChanges();

                var controller = CreateController(context);

                var newReview = new ReviewModel { BookingId = booking.BookingId, Rating = 5, Content = "Great stay!" };

                var result = await controller.PostReview(newReview);

                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var review = Assert.IsType<ReviewModel>(createdAtActionResult.Value);

                Assert.Equal(newReview.Rating, review.Rating);
                Assert.Equal(newReview.Content, review.Content);
            }
        }

        [Fact]
        public async Task PutReview_UpdatesReview()
        {
            int reviewId;

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

                var review = new ReviewModel { BookingId = booking.BookingId, Rating = 5, Content = "Great stay!" };
                context.Reviews.Add(review);
                context.SaveChanges();
                reviewId = review.ReviewId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var updatedReview = new ReviewModel { ReviewId = reviewId, BookingId = context.Bookings.First().BookingId, Rating = 4, Content = "Good stay!" };

                var result = await controller.PutReview(reviewId, updatedReview);

                Assert.IsType<NoContentResult>(result);

                var review = await context.Reviews.FindAsync(reviewId);
                Assert.Equal(updatedReview.Rating, review.Rating);
                Assert.Equal(updatedReview.Content, review.Content);
            }
        }

        [Fact]
        public async Task DeleteReview_DeletesReview()
        {
            int reviewId;

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

                var review = new ReviewModel { BookingId = booking.BookingId, Rating = 5, Content = "Great stay!" };
                context.Reviews.Add(review);
                context.SaveChanges();
                reviewId = review.ReviewId;
            }

            using (var context = new HotelContext(_dbContextOptions, new Mock<IConfiguration>().Object))
            {
                var controller = CreateController(context);

                var result = await controller.DeleteReview(reviewId);

                Assert.IsType<NoContentResult>(result);

                var review = await context.Reviews.FindAsync(reviewId);
                Assert.Null(review);
            }
        }
    }
}
