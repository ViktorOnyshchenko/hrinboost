namespace DL.Models
{
	public class UserModel
	{
		public Guid Id { get; set; }

		public long TelegramId { get; set; }

		public string? UserName { get; set; } = string.Empty;
	}
}
