namespace DL.Models
{
	public class UserHistoryItem
	{
		public Guid Id { get; set; }

		public Guid UserId { get; set; }

		public string CityName { get; set; } = string.Empty;

		public decimal Temperature { get; set; }

		public decimal FeelsLike { get; set; }

		public int Humidity { get; set; }
	}
}
