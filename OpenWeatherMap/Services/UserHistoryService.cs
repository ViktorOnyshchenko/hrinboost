using BLL.Interfaces;
using DL.Interfaces;
using DL.Models;

namespace BLL.Services
{
	public class UserHistoryService : IUserHistoryService
	{
		
		private readonly IUserHistoryRepository _userHistoryRepository;
		private readonly IUserRepository _userRepository;

		public UserHistoryService(IUserHistoryRepository userHistoryRepository, IUserRepository userRepository)
		{
			_userHistoryRepository = userHistoryRepository;
			_userRepository = userRepository;
		}

		public async Task AddAsync(UserModel userModel, WeatherModel weatherModel)
		{
			UserModel? existUser = await _userRepository.GetUserByTelegramIdAsync(userModel.TelegramId);
			if (existUser == null)
			{
				return;
			}

			UserHistoryItem userHistoryItem = new UserHistoryItem() 
			{ 
				UserId = existUser.Id,
				CityName = weatherModel.CityName,
				Temperature = weatherModel.Temperature,
				FeelsLike = weatherModel.FeelsLike,
				Humidity = weatherModel.Humidity
			};

			await _userHistoryRepository.AddAsync(userHistoryItem);
		}
	}
}
