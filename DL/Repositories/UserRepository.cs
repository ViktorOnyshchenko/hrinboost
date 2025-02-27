using Dapper;
using DL.Interfaces;
using DL.Models;
using System.Data;

namespace DL.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly DapperContext _context;

		private const string SQL_Insert = "INSERT INTO Users (TelegramId, UserName) VALUES (@TelegramId, @UserName)";
		private const string SQL_CheckUserByTelegramId = "SELECT COUNT(1) FROM Users WHERE TelegramId LIKE @TelegramId";
		private const string SQL_SelectUserByUserId = "SELECT * FROM Users WHERE Id LIKE @UserId";
		private const string SQL_SelectUserByTelegramId = "SELECT * FROM Users WHERE TelegramId LIKE @TelegramId";

		public UserRepository(DapperContext context)
		{
			_context = context;
		}

		public async Task AddAsync(UserModel user)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("TelegramId", user.TelegramId);
			parameters.Add("UserName", user.UserName);

			using (IDbConnection connection = _context.CreateConnection())
			{
				await connection.QueryAsync(SQL_Insert, parameters);
			}
		}

		public async Task<bool> ExistsAsync(long telegramId)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("TelegramId", telegramId);

			using (IDbConnection connection = _context.CreateConnection())
			{
				return await connection.ExecuteScalarAsync<bool>(SQL_CheckUserByTelegramId, parameters);
			}
		}

		public async Task<UserModel?> GetUserByIdAsync(Guid userId)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("UserId", userId);

			using (IDbConnection connection = _context.CreateConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<UserModel>(SQL_SelectUserByUserId, parameters);
			}
		}

		public async Task<UserModel?> GetUserByTelegramIdAsync(long telegramId)
		{
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("TelegramId", telegramId);

			using (IDbConnection connection = _context.CreateConnection())
			{
				return await connection.QuerySingleOrDefaultAsync<UserModel>(SQL_SelectUserByTelegramId, parameters);
			}
		}
	}
}
