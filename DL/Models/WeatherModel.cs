namespace DL.Models
{
	public class WeatherModel
	{
		public string CityName { get; set; } = string.Empty;

		public decimal Temperature { get; set; }

		public decimal FeelsLike { get; set; }

		public int Humidity { get; set; }
	}
}
