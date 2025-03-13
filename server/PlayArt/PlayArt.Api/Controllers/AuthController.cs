using Api_Bussiness.API.PostEntity;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlayArt.Core.DTOs;
using PlayArt.Core.Interfaces.Services_interfaces;
using PlayArt.Service;
using PlayArt.Sevice;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserService _iuserservice;
    private readonly IMapper _mapper;
    private readonly IUserRoleService _UserRoleService;


    public AuthController(AuthService authService, IUserService iuserservice, IMapper mapper, IUserRoleService userRoleService)
    {
        _authService = authService;
        _iuserservice = iuserservice;
        _mapper = mapper;
        _UserRoleService = userRoleService;
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


    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
    {
        if (model == null)
        {
            return Conflict("User is not valid");
        }
        var modelD = _mapper.Map<UserDTO>(model);
        var existingUser = await _iuserservice.AddUserAsync(modelD);
        if (existingUser == null)
            return BadRequest();
        var userRole = await _UserRoleService.AddAsync(model.RoleName,existingUser.Id);
        if (userRole == null)
            return BadRequest();
        var token = _authService.GenerateJwtToken(model.Email,new[] { model.RoleName });
        return Ok(new { Token = token });
    }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}