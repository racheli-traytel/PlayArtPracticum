using Microsoft.AspNetCore.Mvc;
using PlayArt.Core.Interfaces.Services_interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserService _iuserservice;
    public AuthController(AuthService authService, IUserService iuserservice)
    {
        _authService = authService;
        _iuserservice = iuserservice;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var role = _iuserservice.Authenticate(model.Email, model.Password);
        Console.WriteLine("role",role);
        var user = _iuserservice.GetUserByEmail(model.Email);
        if (role == "Admin")
        {
            var token = _authService.GenerateJwtToken(model.Email, new[] { "Admin" });
            return Ok(new { Token = token,User=user});
        }
        else if (role == "User")
        {
            var token = _authService.GenerateJwtToken(model.Email, new[] { "User" });
            return Ok(new { Token = token,User=user});
        }

        return Unauthorized();
    }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}