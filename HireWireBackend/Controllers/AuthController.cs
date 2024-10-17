using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
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
    public async Task<ActionResult<string>> Register([FromBody] UserDTO userDto)
    {
        var userDb = await _userService.Register(_mapper.Map<User>(userDto));
        var jwt = JwtGenerator.GenerateJwt(userDb, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));
        
        HttpContext.Session.SetInt32("id", userDb.UserId);
        HttpContext.Response.Cookies.Append("token", jwt);

        return Created("token", jwt);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var user = await _userService.Login(email, password);
        var jwt = JwtGenerator.GenerateJwt(user, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));

        HttpContext.Response.Cookies.Append("token", jwt);
        
        return Created("token", jwt);
    }
    
}