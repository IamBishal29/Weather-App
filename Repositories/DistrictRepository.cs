using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public class DistrictRepository : IDistrictRepository
    {
        //if data needed to be saved into database, we can do that by this object.
        //I have tried to not use any database.
        private readonly WeatherDbContext _context;

        public DistrictRepository(WeatherDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets weather data about all districs.
        /// </summary>
        /// <param name="districts"></param>
        /// <returns></returns>
        public async Task<List<District>> GetAllDistrictsAsync(List<District> districts)
        {
            List<District> dList = new List<District>();

            using (HttpClient client = new HttpClient())
            {
                foreach (var district in districts)
                {
                    string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={district.Lat}&longitude={district.Long}&hourly=temperature_2m&timezone=auto";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        var weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResult);

                        // Filter data for 2 PM (hour 14) and calculate the average temperature
                        double averageTemperature = weatherData.Hourly.Time
                            .Where(time => time.Hour == 14)
                            .Select((time, index) => weatherData.Hourly.Temperature_2m[index])
                            .Average();

                        var districtTemperature = new District
                        {
                            Id = district.Id,
                            Name = district.Name,
                            Lat = district.Lat,
                            Long = district.Long,
                            AverageTemperature = averageTemperature,
                            Date = DateTime.Now,
                        };

                        dList.Add(districtTemperature);
                    }
                }

                return dList;
            }
        }

        /// <summary>
        /// Returns a specific district's weather data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="districts"></param>
        /// <returns></returns>
        public async Task<District> GetInfoByDestrictId(int? id,DateTime date, List<District> districts)
        {
            List<District> dList = new List<District>();

            using (HttpClient client = new HttpClient())
            {
                foreach (var district in districts)
                {
                    string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={district.Lat}&longitude={district.Long}&hourly=temperature_2m&timezone=auto";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        var weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResult);

                        // Filter data for 2 PM (hour 14) and calculate the average temperature
                        int indexOf2PM = weatherData.Hourly.Time.FindIndex(time => time.Date == date.Date && time.Hour == 14);

                        // Get the temperature at 2 pm on the current date
                        double temperatureAt2PM = weatherData.Hourly.Temperature_2m[indexOf2PM];


                        var districtTemperature = new District
                        {
                            Id = district.Id,
                            Name = district.Name,
                            Lat = district.Lat,
                            Long = district.Long,
                            //here I considered AverageTemperature as daily. we can have a separate DTO also.
                            AverageTemperature = temperatureAt2PM,
                            Date = DateTime.Now,
                        };

                        dList.Add(districtTemperature);
                    }
                }
                var dailyTemp = dList.FirstOrDefault(district => district.Id == id);
                
                return dailyTemp;
            }
        }
    }
}
