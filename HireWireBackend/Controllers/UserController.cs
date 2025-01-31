using System.Security.Claims;
using HireWireBackend.Core.Interfaces.ILoggers;
using HireWireBackend.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;


[Authorize]
[ApiController]
[Route("api/user")]
public class UserController: Controller
{
    private readonly IUserService _userService;
    private readonly IApplicantService _applicantService;
    private readonly IEmployerService _employerService;
    private readonly IBlobLogger _logger;
    public UserController(IUserService userService, IApplicantService applicantService, IEmployerService employerService, IBlobLogger logger)
    {
        _userService = userService;
        _applicantService = applicantService;
        _employerService = employerService;
        _logger = logger;
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
{
    try
    {
        await _logger.LogAsync("Fetching profile for the current user.", "INFO");

        // Извлекаем userId из токена
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            await _logger.LogAsync("Unauthorized access attempt: User ID not found in token.", "ERROR");
            return Unauthorized("User ID not found in token.");
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            await _logger.LogAsync("Invalid User ID format in token.", "ERROR");
            return BadRequest("Invalid User ID format.");
        }

        await _logger.LogAsync($"Fetching user with ID {userId}.", "INFO");

        // Получаем информацию о пользователе
        var user = await _userService.FindById(userId);
        if (user == null)
        {
            await _logger.LogAsync($"User with ID {userId} not found.", "ERROR");
            return NotFound("User not found.");
        }

        await _logger.LogAsync($"User with ID {userId} found. Role: {user.Role}.", "INFO");

        // Получаем профиль пользователя
        object? profile = null;
        if (user.Role == "Applicant")
        {
            profile = await _applicantService.GetApplicantByUserId(userId);
            await _logger.LogAsync($"Applicant profile fetched for User ID {userId}.", "INFO");
        }
        else if (user.Role == "Employer")
        {
            profile = await _employerService.FindById(userId);
            await _logger.LogAsync($"Employer profile fetched for User ID {userId}.", "INFO");
        }

        // Формируем ответ
        var result = new
        {
            User = new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            },
            Profile = profile
        };

        await _logger.LogAsync($"Profile data successfully fetched for User ID {userId}.", "INFO");
        return Ok(result);
    }
    catch (Exception ex)
    {
        await _logger.LogAsync($"Error fetching profile. Exception: {ex.Message}", "ERROR");
        return BadRequest("Failed to fetch profile.");
    }
}
    
}