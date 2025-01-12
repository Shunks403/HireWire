using System;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Configuration;
using HireWireBackend.Core.Interfaces.ILoggers;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using HireWireBackend.DTO.TokenRequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace HireWireBackend.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IBlobLogger _logger;

    public AuthController(IUserService userService, IConfiguration configuration,IMapper mapper , IBlobLogger logger)
    {
        _userService = userService;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register([FromBody] UserRegistrationDto userDto, [FromServices] IBlobLogger blobLogger)
    {
        try
        {
            // Логируем начало регистрации
            await blobLogger.LogAsync($"Starting registration process for user: {userDto.Email}", "INFO");

            // Регистрируем пользователя
            var userDb = await _userService.Register(_mapper.Map<User>(userDto));
            await blobLogger.LogAsync($"User registered successfully: {userDto.Email}", "INFO");

            // Генерация Access-токена и Refresh-токена
            var jwt = JwtGenerator.GenerateJwt(userDb, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(1));
            var refreshToken = JwtGenerator.GenerateRefreshToken();
            await blobLogger.LogAsync($"JWT and Refresh Token generated for user: {userDto.Email}", "INFO");

            // Сохранение Refresh-токена в базе данных
            userDb.RefreshToken = refreshToken;
            userDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh-токен живёт 7 дней
            await _userService.Update(userDb);
            await blobLogger.LogAsync($"Refresh Token saved to database for user: {userDto.Email}", "INFO");

            // Возвращаем успешный ответ
            return Ok(new
            {
                AccessToken = jwt,
                RefreshToken = refreshToken
            });
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            await blobLogger.LogAsync($"Error occurred during registration process for user: {userDto?.Email}. Exception: {ex.Message}", "ERROR");
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    [HttpPost("login")]
public async Task<ActionResult<string>> Login([FromQuery] string email, [FromQuery] string password, [FromServices] IBlobLogger blobLogger)
{
    try
    {
        // Логируем начало процесса входа
        await blobLogger.LogAsync($"Login attempt for email: {email}", "INFO");


        // Проверка входных данных
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await blobLogger.LogAsync("Login failed: Email or password is missing.", "ERROR");
            return BadRequest("Email and password are required.");
        }

        // Аутентификация пользователя
        var user = await _userService.Login(email, password);
        if (user == null)
        {
            await blobLogger.LogAsync($"Login failed: Invalid credentials for email: {email}", "ERROR");
            return Unauthorized("Invalid email or password.");
        }
        await blobLogger.LogAsync($"User authenticated successfully: {email}", "INFO");

        // Генерация Access-токена и Refresh-токена
        var jwt = JwtGenerator.GenerateJwt(user, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var refreshToken = JwtGenerator.GenerateRefreshToken();
        await blobLogger.LogAsync($"JWT and Refresh Token generated for email: {email}", "INFO");

        // Сохранение Refresh-токена
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userService.Update(user);
        await blobLogger.LogAsync($"Refresh Token saved to database for user: {email}", "INFO");

        // Возвращаем успешный ответ
        return Ok(new
        {
            AccessToken = jwt,
            RefreshToken = refreshToken
        });
    }
    catch (Exception ex)
    {
        // Логируем ошибку
        await blobLogger.LogAsync($"Unexpected error during login for email: {email}. Exception: {ex.Message}", "ERROR");
        return StatusCode(500, "An error occurred during login.");
    }
}
    
    
    [HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest, [FromServices] IBlobLogger blobLogger)
{
    try
    {
        // Логируем начало запроса
        await blobLogger.LogAsync("Refresh token request received.", "INFO");

        // Проверяем, предоставлен ли Refresh Token
        if (string.IsNullOrEmpty(tokenRequest.RefreshToken))
        {
            await blobLogger.LogAsync("Refresh token request failed: Refresh Token is missing.", "ERROR");
            return BadRequest("Refresh Token is required.");
        }

        // Проверяем Refresh Token в базе данных
        var user = await _userService.GetUserByRefreshToken(tokenRequest.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            await blobLogger.LogAsync($"Refresh token request failed: Invalid or expired token: {tokenRequest.RefreshToken}", "ERROR");
            return Unauthorized("Invalid or expired Refresh Token.");
        }

        // Логируем успешную проверку Refresh Token
        await blobLogger.LogAsync($"Refresh token validated successfully for user: {user.Email}.", "INFO");

        // Генерируем новые токены
        var newAccessToken = JwtGenerator.GenerateJwt(user, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var newRefreshToken = JwtGenerator.GenerateRefreshToken();
        await blobLogger.LogAsync($"New JWT and Refresh Token generated for user: {user.Email}.", "INFO");

        // Обновляем Refresh Token у пользователя
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userService.Update(user);
        await blobLogger.LogAsync($"Refresh token updated in database for user: {user.Email}.", "INFO");

        // Возвращаем успешный ответ
        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
    catch (Exception ex)
    {
        // Логируем общую ошибку
        await blobLogger.LogAsync($"Unexpected error during refresh token process. Exception: {ex.Message}", "ERROR");
        return StatusCode(500, "An error occurred while processing the refresh token.");
    }
}
}