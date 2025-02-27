using BLL.Interfaces;
using Dapper;
using DL;
using DL.Interfaces;
using DL.Models;
using DL.Repositories;
using System.Data;

namespace BLL.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IUserHistoryRepository _userHistoryRepository;

		public UserService(IUserRepository userRepository, IUserHistoryRepository userHistoryRepository)
		{
			_userRepository = userRepository;
			_userHistoryRepository = userHistoryRepository;
		}

		public async Task AddAsync(UserModel user)
		{
			bool isExist = await _userRepository.ExistsAsync(user.TelegramId);

			if (!isExist)
			{
				await _userRepository.AddAsync(user);
			}
		}

		public async Task<UserModel?> GetUserByIdAsync(Guid userId)
		{
			return await _userRepository.GetUserByIdAsync(userId);
		}

		public async Task<UserDetailedModel?> GetUserWithHistoryByIdAsync(Guid userId)
		{
			UserDetailedModel? userDetailedModel = null;
			UserModel? userModel = await _userRepository.GetUserByIdAsync(userId);

			if (userModel == null)
			{
				return userDetailedModel;
			}

			userDetailedModel = new UserDetailedModel();
			userDetailedModel.User = userModel;

			IEnumerable<UserHistoryItem> userHistoryItems = await _userHistoryRepository.GetByUserIdAsync(userDetailedModel.User.Id);

			if (userHistoryItems.Any())
			{
				foreach (UserHistoryItem item in userHistoryItems)
				{
					userDetailedModel.WeatherHistory.Add(new WeatherModel
					{
						CityName = item.CityName,
						Temperature = item.Temperature,
						FeelsLike = item.FeelsLike,
						Humidity = item.Humidity
					});
				}
			}

			return userDetailedModel;
		}
	}
}
