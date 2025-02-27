using DL.Models;

namespace DL.Interfaces
{
	public interface IUserHistoryRepository
	{
		public Task AddAsync(UserHistoryItem userHistoryItem);

		public Task<IEnumerable<UserHistoryItem>> GetByUserIdAsync(Guid userId);

	}
}
