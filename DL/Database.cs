using Dapper;
using System.Data;

namespace DL
{
	public class Database
	{
		private readonly DapperContext _context;

		public Database(DapperContext context)
		{
			_context = context;
		}

		public void CreateDatabase(string dbName)
		{
			string query = "SELECT * FROM sys.databases WHERE name = @name";
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("name", dbName);
			using (IDbConnection connection = _context.CreateMasterConnection())
			{
				IEnumerable<dynamic> records = connection.Query(query, parameters);

				if (!records.Any())
				{
					connection.Execute($"CREATE DATABASE {dbName}");
				}

				CreateTables();
			}
		}

		private void CreateTables()
		{
			using (IDbConnection connection = _context.CreateConnection())
			{
				string createUsers = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
				BEGIN
					CREATE TABLE Users (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
					TelegramId BIGINT NOT NULL,
                    UserName VARCHAR(30),
					)
				END
				
				IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserHistory]') AND type in (N'U'))
				BEGIN
					CREATE TABLE UserHistory (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
					UserId UNIQUEIDENTIFIER NOT NULL,
                    CityName VARCHAR(30) NOT NULL,
					Temperature DECIMAL(4, 2) NOT NULL,
					FeelsLike DECIMAL(4, 2) NOT NULL,
					Humidity INT NOT NULL,
					FOREIGN KEY (UserId) REFERENCES Users(Id)
					)
				END";

				connection.Execute(createUsers);
			}
		}
	}
}
