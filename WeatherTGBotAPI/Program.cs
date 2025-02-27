using BLL.Interfaces;
using BLL.Services;
using DL;
using DL.Interfaces;
using DL.Repositories;
using WeatherTGBotAPI.Extensions;

namespace WeatherTGBot
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddSingleton<DapperContext>();
			builder.Services.AddSingleton<Database>();

			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IUserHistoryService, UserHistoryService>();
			builder.Services.AddScoped<IUserHistoryRepository, UserHistoryRepository>();

			WebApplication app = builder
						.Build()
						.MigrateDatabase();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
