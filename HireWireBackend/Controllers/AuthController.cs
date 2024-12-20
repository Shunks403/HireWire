using System;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Configuration;
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
        var jwt = JwtGenerator.GenerateJwt(userDb, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(1));
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
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
    {
        if (string.IsNullOrEmpty(tokenRequest.RefreshToken))
        {
            return BadRequest("Refresh Token is required.");
        }

        var user = await _userService.GetUserByRefreshToken(tokenRequest.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired Refresh Token.");
        }

        // Генерируем новые токены
        var newAccessToken = JwtGenerator.GenerateJwt(user, _configuration["TokenKey"], DateTime.UtcNow.AddMinutes(5));
        var newRefreshToken = JwtGenerator.GenerateRefreshToken();

        // Обновляем Refresh Token у пользователя
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userService.Update(user);

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}