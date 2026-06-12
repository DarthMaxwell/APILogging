using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<IdentityUser> _user;
    private readonly SignInManager<IdentityUser> _login;
     private readonly IConfiguration _config;
    

    public AuthenticationController(ILogger<AuthenticationController> logger, UserManager<IdentityUser> user, SignInManager<IdentityUser> login, IConfiguration config)
    {
        _logger = logger;
        _user = user;
        _login = login;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO regdto)
    {
        if (regdto.Password != regdto.ConfirmPassword)
        {
            return BadRequest("Passwords do not match");
        }

        IdentityUser? isUser = await _user.FindByNameAsync(regdto.Username);
        if (isUser != null)
        {
            return Conflict("Username already taken");
        }

        IdentityUser newUser = new IdentityUser
        {
            UserName = regdto.Username
        };

        IdentityResult result = await _user.CreateAsync(newUser, regdto.Password);

        if (result.Succeeded)
        {
            return Created($"/api/{newUser.Id}", new { message = "User registered successfully." });
        }
        
        IEnumerable<string> error = result.Errors.Select(e => e.Description);
        return BadRequest(error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO logindto)
    {
        IdentityUser? user = await _user.FindByNameAsync(logindto.Username);

        if (user == null)
        {
            return Unauthorized("Invalid Credentials");
        }

        var result = await _login.CheckPasswordSignInAsync(user, logindto.Password, true);

        if (result.IsLockedOut)
        {
            return StatusCode(423, "Account locked out");
        }

        if (result.Succeeded)
        {
            string token = GenerateJwt(user);
            return Ok(new {token});
        }

        return Unauthorized("Invalid Credentials");
    }

    [HttpGet("auth")]
    [Authorize]
    public async Task<IActionResult> CheckAuth()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        return Ok(new {username});
    }

    private string GenerateJwt(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}