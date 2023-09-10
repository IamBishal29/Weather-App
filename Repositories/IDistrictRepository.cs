using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public interface IDistrictRepository
    {
        /// <summary>
        /// Gets weather data about all districs.
        /// </summary>
        /// <param name="districts"></param>
        /// <returns></returns>
        Task<List<District>> GetAllDistrictsAsync(List<District> districts);

        /// <summary>
        /// Returns a specific district's weather data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="districts"></param>
        /// <returns></returns>
        Task<District> GetInfoByDestrictId(int ?id, DateTime date, List<District> districts);
    }
}
