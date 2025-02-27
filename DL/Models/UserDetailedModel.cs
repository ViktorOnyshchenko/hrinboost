namespace DL.Models
{
	public class UserDetailedModel
    {
        public UserModel? User { get; set; } = null;

        public ICollection<WeatherModel> WeatherHistory { get; set; } = new List<WeatherModel>();
    }
}
