using BLL.Interfaces;
using DL.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BLL.Services
{
	public class OpenWeatherMapService : IOpenWeatherMapService
	{
		private const string DefaultUrl = "https://api.openweathermap.org/data/2.5/";
		private readonly string APIKey;
		private readonly HttpClient client;

		public OpenWeatherMapService(IConfiguration configuration)
		{
			APIKey = configuration.GetValue<string>("OpenWeatherMap") ?? string.Empty;
			client = new HttpClient();
			client.BaseAddress = new Uri(DefaultUrl);
		}

		public async Task<WeatherModel?> GetWeatherByCityNameAsync(string cityName)
		{
			HttpResponseMessage response = await client.GetAsync($"weather?q={cityName}&units=metric&appid={APIKey}");

			string content = await response.Content.ReadAsStringAsync();
			dynamic? responseObj = JsonConvert.DeserializeObject(content);

			if (!response.IsSuccessStatusCode)
			{
				string errorMessage = responseObj.message;
				throw new ArgumentException($"Error message: {errorMessage[0].ToString().ToUpper()}{errorMessage.Substring(1)}.");
			}

			WeatherModel? weatherModel = null;

			if (responseObj is not null)
			{
				weatherModel = new WeatherModel()
				{
					CityName = responseObj.name,
					Temperature = responseObj.main.temp,
					FeelsLike = responseObj.main.feels_like,
					Humidity = responseObj.main.humidity
				};
			}

			return weatherModel;
		}
	}
}
