using Newtonsoft.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class DistrictDataService
    {
        /// <summary>
        /// Read and parse the JSON file into a list of district objects
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns>JsonConvert.DeserializeObject<List<District>>(jsonString);</returns>
        public List<District> GetDistrictsFromJson(string jsonString)
        {
            var districtData = JsonConvert.DeserializeObject<DistrictData>(jsonString);
            return districtData.Districts;
        }
    }
}
