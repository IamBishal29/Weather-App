namespace WeatherAPI.Models
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double AverageTemperature { get; set; }
        public DateTime Date { get; set; }
    }
}
