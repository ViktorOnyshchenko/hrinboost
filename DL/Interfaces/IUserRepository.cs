using DL.Models;

namespace DL.Interfaces
{
	public interface IUserRepository
	{
		public Task AddAsync(UserModel user);

		public Task<bool> ExistsAsync(long telegramId);

		public Task<UserModel?> GetUserByIdAsync(Guid userId);

		public Task<UserModel?> GetUserByTelegramIdAsync(long telegramId);
	}
}
