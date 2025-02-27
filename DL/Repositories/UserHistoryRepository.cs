using Dapper;
using DL.Interfaces;
using DL.Models;
using System.Data;

namespace DL.Repositories
{
	public class UserHistoryRepository : IUserHistoryRepository
	{
		private readonly DapperContext _context;

		private const string SQL_Insert = @"
			INSERT INTO UserHistory (UserId, CityName, Temperature, FeelsLike, Humidity) 
			VALUES (@UserId, @CityName, @Temperature, @FeelsLike, @Humidity)";

		private const string SQL_SelectUserByUserId = "SELECT TOP(@MaxLimit) * FROM UserHistory WHERE UserId LIKE @UserId";

		private const int MaxLimit = 5;

		public UserHistoryRepository(DapperContext context)
		{
			_context = context;
		}

		public async Task AddAsync(UserHistoryItem userHistoryItem)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("UserId", userHistoryItem.UserId);
			parameters.Add("CityName", userHistoryItem.CityName);
			parameters.Add("Temperature", userHistoryItem.Temperature);
			parameters.Add("FeelsLike", userHistoryItem.FeelsLike);
			parameters.Add("Humidity", userHistoryItem.Humidity);

			using (IDbConnection connection = _context.CreateConnection())
			{
				await connection.QueryAsync(SQL_Insert, parameters);
			}
		}

		public async Task<IEnumerable<UserHistoryItem>> GetByUserIdAsync(Guid userId)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("UserId", userId);
			parameters.Add("MaxLimit", MaxLimit);

			using (IDbConnection connection = _context.CreateConnection())
			{
				return await connection.QueryAsync<UserHistoryItem>(SQL_SelectUserByUserId, parameters);
			}
		}
	}
}
