using BLL.Interfaces;
using DL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WeatherTGBotAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> Get([FromRoute] Guid userId)
		{
			UserModel? userModel = await _userService.GetUserByIdAsync(userId);
			
			if (userModel == null)
			{
				return NotFound();
			}

			return Ok(userModel);
		}

		[HttpGet("{userId}/withHistory")]
		public async Task<IActionResult> GetUserWithHistory([FromRoute] Guid userId)
		{
			UserDetailedModel? userModel = await _userService.GetUserWithHistoryByIdAsync(userId);

			if (userModel == null)
			{
				return NotFound();
			}

			return Ok(userModel);
		}

		[HttpGet("SendWeatherToAll")]
		public IEnumerable<UserModel> SendWeatherToAll()
		{
			return new List<UserModel>() { new UserModel() { UserName = "Viktor" } };
		}
	}
}
