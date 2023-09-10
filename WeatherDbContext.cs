using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;

namespace WeatherAPI
{
    public class WeatherDbContext : DbContext
    {
        public DbSet<District> Districts { get; set; }
        // Add other DbSets as needed

        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
            : base(options)
        {
        }
    }
}
