using DL.Models;

namespace BLL.Interfaces
{
	public interface IUserService
    {
        public Task<UserModel?> GetUserByIdAsync(Guid userId);

		public Task<UserDetailedModel?> GetUserWithHistoryByIdAsync(Guid userId);

		public Task AddAsync(UserModel user);
    }
}
