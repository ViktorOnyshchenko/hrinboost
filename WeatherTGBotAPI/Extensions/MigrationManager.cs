using DL;

namespace WeatherTGBotAPI.Extensions
{
	public static class MigrationManager
	{
		public static WebApplication MigrateDatabase(this WebApplication webApp)
		{
			using (var scope = webApp.Services.CreateScope())
			{
				var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
				try
				{
					databaseService.CreateDatabase("weatherTGBotDb");
				}
				catch
				{
					throw;
				}
			}
			return webApp;
		}
	}
}
