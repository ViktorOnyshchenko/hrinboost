using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using BLL.Interfaces;
using BLL.Services;
using DL;
using Microsoft.Extensions.Configuration;
using DL.Models;
using DL.Interfaces;
using DL.Repositories;

namespace WeatherTGBot
{
	internal class Program
	{
		private static Dictionary<string, string> cities = new Dictionary<string, string>()
		{
			{ "1", "Kyiv" },
			{ "2", "Dnipro" },
		};

		private static IConfiguration _configuration;
		private static DapperContext _dapperContext;
		private static IUserRepository _userRepository;
		private static IUserHistoryRepository _userHistoryRepository;
		private static IUserService _userService;
		private static IUserHistoryService _userHistoryService;


		// Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
		private static ITelegramBotClient _botClient;

		// Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
		private static ReceiverOptions _receiverOptions;

		private static string TGBotKey;

		public static async Task Main()
		{
			Init();

			try
			{
				_botClient = new TelegramBotClient(TGBotKey); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
				_receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
				{
					AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
					{
					UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
					UpdateType.CallbackQuery // Inline кнопки
				},
					// Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
					// True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
					DropPendingUpdates = true
				};

				using var cts = new CancellationTokenSource();

				// UpdateHander - обработчик приходящих Update`ов
				// ErrorHandler - обработчик ошибок, связанных с Bot API
				_botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

				User me = await _botClient.GetMe(); // Создаем переменную, в которую помещаем информацию о нашем боте.
				Console.WriteLine($"{me.FirstName} started!");

				await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
			}
			catch (Exception ex)
			{
                Console.WriteLine($"Error: {ex.Message}");
			}
		}

		private static void Init()
		{
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets<Program>()
				.Build();

			TGBotKey = _configuration.GetValue<string>("TGBotKey");

			_dapperContext = new DapperContext(_configuration);
			_userRepository = new UserRepository(_dapperContext);
			_userHistoryRepository = new UserHistoryRepository(_dapperContext);
			_userService = new UserService(_userRepository, _userHistoryRepository);
			_userHistoryService = new UserHistoryService(_userHistoryRepository, _userRepository);
		}

		private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			// Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
			try
			{
				// Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
				switch (update.Type)
				{
					case UpdateType.Message:
						{
							// эта переменная будет содержать в себе все связанное с сообщениями
							Message? message = update.Message;

							// Chat - содержит всю информацию о чате
							Chat chat = message.Chat;

							// From - это от кого пришло сообщение (или любой другой Update)
							User? user = message.From;

							string[]? argsArray = message.Text?.Split(' ');

							if (argsArray is null || argsArray.Length <= 0 || !argsArray[0].StartsWith('/'))
							{
								await SendDefaultMessageAsync(botClient, chat, argsArray.Length > 0);

								return;
							}

							switch (argsArray[0].Substring(1))
							{
								case "weather":
									{
										if (string.IsNullOrWhiteSpace(argsArray[1]))
										{
											await botClient.SendMessage(
												chat.Id,
												"Incorrect city",
												ParseMode.Html);
										}

										string cityName = argsArray[1];

										await SendWeatherToUserAsync(botClient, chat, user, cityName);

										break;
									}
								default:
									{
										// Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
										Console.WriteLine($"{user.FirstName} ({user.Id}) wrote message: {message.Text}");
										await SendDefaultMessageAsync(botClient, chat);

										break;
									}
							}

							return;
						}
					case UpdateType.CallbackQuery:
						{
							// Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
							CallbackQuery? callbackQuery = update.CallbackQuery;

							// Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
							User user = callbackQuery.From;

							// Вот тут нужно уже быть немножко внимательным и не путаться!
							// Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
							// кнопка привязана к сообщению, то мы берем информацию от сообщения.
							Chat chat = callbackQuery.Message.Chat;

							string cityName = cities[callbackQuery.Data];

							// Выводим на экран нажатие кнопки
							Console.WriteLine($"{user.FirstName} ({user.Id}) pressed button: {callbackQuery.Data}");

							// В этом типе клавиатуры обязательно нужно использовать следующий метод
							await botClient.AnswerCallbackQuery(callbackQuery.Id);
							// Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

							await SendWeatherToUserAsync(botClient, chat, user, cityName);

							return;
						}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
		{
			// Тут создадим переменную, в которую поместим код ошибки и её сообщение 
			var ErrorMessage = error switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => error.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}

		private static async Task SendWeatherToUserAsync(ITelegramBotClient botClient, Chat chat, User? user, string cityName)
		{
			IOpenWeatherMapService openWeatherMapService = new OpenWeatherMapService(_configuration);
			string? message = null;
			UserModel? userModel = null;

			try
			{
				WeatherModel? weather = await openWeatherMapService.GetWeatherByCityNameAsync(cityName);

				if (user != null)
				{
					userModel = new UserModel { TelegramId = user.Id, UserName = user.Username };
					await SaveUserAsync(userModel);
				}

				if (userModel != null && weather != null)
				{
					await SaveUserHistoryAsync(userModel, weather);
					message = $"<b>{weather.CityName}</b>\nTempereature: {weather.Temperature}\nFeels like: {weather.FeelsLike}\nHumidity: {weather.Humidity}";
				}
				else
				{
					message = "No data.";
				}

			}
			catch (ArgumentException ex)
			{
				message = ex.Message;
			}
			finally
			{
				await botClient.SendMessage(
					chat.Id,
					message ?? "Something goes wrong. Try later.",
					ParseMode.Html);
			}
		}

		private static async Task SendDefaultMessageAsync(ITelegramBotClient botClient, Chat chat, bool isIncorrect = false)
		{
			List<InlineKeyboardButton> inlineKeyboardButtons = new List<InlineKeyboardButton>();

			// Dynamically create buttons from the cities dictionary
			foreach (KeyValuePair<string, string> city in cities)
			{
				// Create a new row for each button
				inlineKeyboardButtons.Add(InlineKeyboardButton.WithCallbackData(city.Value, city.Key));
			}

			var inlineKeyboard = new InlineKeyboardMarkup(inlineKeyboardButtons);

			if (isIncorrect)
			{
				await botClient.SendMessage(
					chat.Id,
					"Incorrect input, please, follow instructions.",
					ParseMode.Html);
			}

			await botClient.SendMessage(
					chat.Id,
					"You can enter command like /weather \"city name\" or use buttons.",
					ParseMode.Html);

			await botClient.SendMessage(
					chat.Id,
					"Choose city to get weather:",
					replyMarkup: inlineKeyboard);
		}

		private static async Task SaveUserAsync(UserModel userModel)
		{
			await _userService.AddAsync(userModel);
		}

		private static async Task SaveUserHistoryAsync(UserModel userModel, WeatherModel weatherModel)
		{
			await _userHistoryService.AddAsync(userModel, weatherModel);
		}
	}
}
