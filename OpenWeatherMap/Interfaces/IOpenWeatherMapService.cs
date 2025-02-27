using DL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
	public interface IOpenWeatherMapService
	{
		Task<WeatherModel?> GetWeatherByCityNameAsync(string cityName);
	}
}
