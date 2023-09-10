namespace WeatherAPI.Models
{
    public class HourlyData
    {
        public List<DateTime> Time { get; set; }
        public List<double> Temperature_2m { get; set; }
    }
}
