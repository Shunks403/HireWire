using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using HireWireBackend.DTO.TokenRequestDto;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthController(IUserService userService, IConfiguration configuration,IMapper mapper)
    {
        _userService = userService;
        _configuration = configuration;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register([FromBody] UserRegistrationDto userDto)
    {
        var userDb = await _userService.Register(_mapper.Map<User>(userDto));
    
        // Генерация Access-токена и Refresh-токена
        var jwt = JwtGenerator.GenerateJwt(userDb, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var refreshToken = JwtGenerator.GenerateRefreshToken();

        // Сохранение Refresh-токена в базе данных
        userDb.RefreshToken = refreshToken;
        userDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh-токен живёт 7 дней
        await _userService.Update(userDb);

        return Ok(new
        {
            AccessToken = jwt,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var user = await _userService.Login(email, password);
    
        // Генерация Access-токена и Refresh-токена
        var jwt = JwtGenerator.GenerateJwt(user, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var refreshToken = JwtGenerator.GenerateRefreshToken();

        // Сохранение Refresh-токена
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userService.Update(user);

        return Ok(new
        {
            AccessToken = jwt,
            RefreshToken = refreshToken
        });
    }
    
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken()
    {
        // Получаем refresh-токен из HttpOnly cookie
        var refreshToken = Request.Cookies["refreshToken"];

        // Проверяем токен и его срок действия
        var user = await _userService.GetUserByRefreshToken(refreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        // Генерация нового Access-токена и Refresh-токена
        var newAccessToken = JwtGenerator.GenerateJwt(user, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var newRefreshToken = JwtGenerator.GenerateRefreshToken();

        // Обновляем Refresh-токен у пользователя в базе
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userService.Update(user);

        // Сохраняем новый refresh-токен в HttpOnly cookie
        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Только через HTTPS
            SameSite = SameSiteMode.Strict, // Защита от CSRF
            Expires = DateTimeOffset.UtcNow.AddDays(7) // Срок действия
        });

        // Возвращаем только Access-токен
        return Ok(new
        {
            AccessToken = newAccessToken
        });
    }
}