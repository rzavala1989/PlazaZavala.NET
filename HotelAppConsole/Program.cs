using HotelApp.DataAccess.Context;
using HotelAppDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelApp.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<HotelContext>())
            {
                context.Database.Migrate(); // Ensure the database is created and the latest migration is applied

                // Check if the database is already seeded
                if (!context.Hotels.Any())
                {
                    SeedDatabase(context);
                    System.Console.WriteLine("Database setup and sample data added!");
                
                }
                else
                {
                    System.Console.WriteLine("Database already seeded!");
                }
            }
        }

       private static void SeedDatabase(HotelContext context)
{
    // 1. Create Hotels
    var hotels = new List<HotelModel>
    {
        new HotelModel { Name = "Grand Plaza", Address = "123 Grand Ave" },
        new HotelModel { Name = "Ocean View", Address = "456 Ocean Drive" }
    };
    context.Hotels.AddRange(hotels);
    context.SaveChanges(); // Save here to generate IDs for hotels

    // 2. Create Room Types
    var roomTypes = new List<RoomTypeModel>
    {
        new RoomTypeModel { TypeName = "Standard", Description = "A standard room with basic amenities.", Price = 100 },
        new RoomTypeModel { TypeName = "Deluxe", Description = "A deluxe room with ocean views and a minibar.", Price = 200 },
        new RoomTypeModel { TypeName = "Suite", Description = "A spacious suite with a separate living area and premium amenities.", Price = 300 }
    };
    context.RoomTypes.AddRange(roomTypes);
    context.SaveChanges(); // Save here to generate IDs for room types

    // 3. Create Rooms
    var rooms = new List<RoomModel>();
    foreach (var roomType in roomTypes)
    {
        for (int i = 1; i <= 3; i++) // Creating 3 rooms per room type
        {
            rooms.Add(new RoomModel
            {
                RoomNumber = $"{roomType.TypeName.Substring(0, 1)}{i:00}",
                RoomTypeId = roomType.RoomTypeId
            });
        }
    }
    context.Rooms.AddRange(rooms);
    context.SaveChanges(); // Save here to generate IDs for rooms

    // 4. Create Guests
    var guests = new List<GuestModel>
    {
        new GuestModel { FirstName = "Mickey Mouse", LastName = "Doe", Phone = "123-456-7890", Email = "mickthetrick@example.com" },
        new GuestModel { FirstName = "Jane", LastName = "Eyre", Phone = "987-654-3210", Email = "boringgirl@example.com" },
        new GuestModel { FirstName = "Jim", LastName = "Beam", Phone = "555-555-5555", Email = "jim.beam@example.com" },
        new GuestModel { FirstName = "Jack", LastName = "Daniels", Phone = "666-666-6666", Email = "jack.daniels@example.com" }
    };
    context.Guests.AddRange(guests);
    context.SaveChanges(); // Save here to generate IDs for guests

    // 5. Create Bookings
    var bookingList = new List<BookingModel>();
    // Example booking
    bookingList.Add(new BookingModel
    {
        RoomTypeId = roomTypes.First().RoomTypeId,
        RoomId = rooms.First().RoomId,
        HotelId = hotels.First().HotelId,
        GuestId = guests.First().GuestModelId,
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(1)
    });
    // Add more bookings as needed
    bookingList.Add(new BookingModel
    {
        RoomTypeId = roomTypes.Last().RoomTypeId,
        RoomId = rooms[6].RoomId,
        HotelId = hotels.Last().HotelId,
        GuestId = guests.Last().GuestModelId,
        StartDate = DateTime.UtcNow.AddDays(2),
        EndDate = DateTime.UtcNow.AddDays(4)
    });
        bookingList.Add(new BookingModel
        {
            RoomTypeId = roomTypes.Last().RoomTypeId,
            RoomId = rooms[7].RoomId,
            HotelId = hotels.Last().HotelId,
            GuestId = guests[1].GuestModelId,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(7)
        });
        bookingList.Add(new BookingModel
    {
        RoomTypeId = roomTypes.First().RoomTypeId,
        RoomId = rooms[2].RoomId,
        HotelId = hotels.First().HotelId,
        GuestId = guests[2].GuestModelId,
        StartDate = DateTime.UtcNow.AddDays(8),
        EndDate = DateTime.UtcNow.AddDays(10)
    });
    context.Bookings.AddRange(bookingList);
    context.SaveChanges(); // Save here to generate IDs for bookings

    // 6. Create Reviews
    var reviews = new List<ReviewModel>
    {
        new ReviewModel
        {
            BookingId = bookingList.First().BookingId,
            Rating = 5,
            Content = "Excellent stay, highly recommended!"
        },
        // Add more reviews as needed
    };
    context.Reviews.AddRange(reviews);
    context.SaveChanges(); // Final save to persist reviews
}

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<HotelContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
