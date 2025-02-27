using DL.Models;

namespace BLL.Interfaces
{
	public interface IUserHistoryService
    {
        public Task AddAsync(UserModel userModel, WeatherModel weatherModel);
    }
}
