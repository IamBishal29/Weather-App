using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WeatherAPI.DTO_s;
using WeatherAPI.Models;
using WeatherAPI.Repositories;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly DistrictDataService _districtDataService;
        //JSON file url
        private const string _fileUrl = "https://raw.githubusercontent.com/strativ-dev/technical-screening-test/main/bd-districts.json";

        /// <summary>
        /// Inject all the dependencies into the constructor.
        /// </summary>
        /// <param name="districtRepository"></param>
        /// <param name="districtDataService"></param>
        public DistrictController(IDistrictRepository districtRepository, DistrictDataService districtDataService)
        {
            _districtRepository = districtRepository;
            _districtDataService = districtDataService;
        }

        /// <summary>
        /// Get the top 10 coolest districts based on average temperature.
        /// </summary>
        /// <remarks>
        /// Retrieves the districts with the lowest average temperature.
        /// </remarks>
        /// <returns>A list of District objects representing the coolest districts.</returns>
        /// <response code="200">Returns the top 10 coolest districts.</response>
        /// <response code="500">If an error occurs while retrieving the data.</response>
        [HttpGet("TopTenCoolest")]
        [ProducesResponseType(typeof(List<District>), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCoolestDistricts()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_fileUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                   
                    //Gets all district data from JSON file
                    var districts = _districtDataService.GetDistrictsFromJson(jsonString);

                    //Gets all 64 district weather data
                    var finalDta = await _districtRepository.GetAllDistrictsAsync(districts);

                    //Only return the coolest 10
                    var coolestDistricts = finalDta.OrderBy(district => district.AverageTemperature)
                            .Take(10)
                            .ToList();

                    return Ok(coolestDistricts);
                }
                else
                {
                    return BadRequest("Failed to fetch the JSON file.");
                }
            }
        }

        /// <summary>
        /// Provides suggestions. 
        /// I have used POST method as i wanted to read from request body.
        /// GET also can be used here. In that case need to retrive data from Request header.
        /// </summary>
        /// <param name="request">TravelRequestDTO</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(TravelRequestDTO), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [HttpPost("ShouldTravelOrNot")]
        public async Task<IActionResult> GetTravelRecommendation(TravelRequestDTO request)
        {
            // Implement the logic to retrieve temperature data and calculate the recommendation
            var travelRequestResponse = new TravelResponseDTO();
            
            if(request.Origin.IsNullOrEmpty() || request.Destination.IsNullOrEmpty())
            {
                return BadRequest("No input can not be null or empty");
            }

            if(request.TravelDate.Date < DateTime.Now.Date)
            {
                return BadRequest("Invalid travel date");
            }

            //we can add more validations here as per business needs 

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_fileUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    //As I am not using any Database i need to load data in every request.I used inmemory oprerations.
                    //If we use a separate database then we need not to do that
                    //To keep the temperature data up-to-date, we can set up a periodic task using a scheduler library like Hangfire or a scheduled Azure Function.
                    //This task would periodically fetch new temperature data for all districts and update the database accordingly.
                    var districts = _districtDataService.GetDistrictsFromJson(jsonString);

                    // Check if any District's Name property matches the request string
                    bool isMatchOrigin = districts.Any(district => district.Name.Equals(request.Origin, StringComparison.OrdinalIgnoreCase));
                    bool isMatchDestination = districts.Any(district => district.Name.Equals(request.Destination, StringComparison.OrdinalIgnoreCase));
                    
                    if (!isMatchOrigin || !isMatchDestination)
                    {
                        return BadRequest("Invalid District name");
                    }

                    int ?matchingOriginId = districts
                                            .Where(district => district.Name.Equals(request.Origin, StringComparison.OrdinalIgnoreCase))
                                            .Select(district => (int?)district.Id)
                                            .FirstOrDefault();

                    var origin = await _districtRepository.GetInfoByDestrictId(matchingOriginId,request.TravelDate, districts);

                    int? matchingDestinationId = districts
                                            .Where(district => district.Name.Equals(request.Destination, StringComparison.OrdinalIgnoreCase))
                                            .Select(district => (int?)district.Id)
                                            .FirstOrDefault();

                    var destination = await _districtRepository.GetInfoByDestrictId(matchingDestinationId, request.TravelDate, districts);

                    if(destination == null || origin == null) { return BadRequest("Inavlid request"); }

                    //I have used AverageTemperature variable to store daily temperature to save development time
                    // We can use a different model or DTO here for better readability.This is an improvement part.
                    travelRequestResponse.OriginTemperature = origin.AverageTemperature;
                    travelRequestResponse.DestinationTemperature = destination.AverageTemperature;

                    if (origin.AverageTemperature > destination.AverageTemperature)
                    {
                        travelRequestResponse.Recommendation = "Travel is recommended";
                    }
                    
                    else
                    {
                        travelRequestResponse.Recommendation = "Travel is not recommended";
                    }
                }
                else
                {
                    return BadRequest("Failed to fetch the JSON file.");
                }
            }

            return Ok(travelRequestResponse);
        }
    }
}
