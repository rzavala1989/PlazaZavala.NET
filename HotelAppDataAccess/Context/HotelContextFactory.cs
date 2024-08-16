using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HotelApp.DataAccess.Context;

public class HotelContextFactory : IDesignTimeDbContextFactory<HotelContext>
{
    public HotelContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotelContext>();

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Get connection string from appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Configure the context to use Npgsql
        optionsBuilder.UseNpgsql(connectionString);

        return new HotelContext(optionsBuilder.Options, configuration);
    }
}