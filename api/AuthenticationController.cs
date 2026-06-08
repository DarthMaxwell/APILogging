using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO regdto)
    {
        //check if user dosnt exist
        //check if password and confirm password match
        
        return Created("idk what this is for", $"User registered: {regdto.Username}");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO logindto)
    {
        // if can log in as that user
        // return token
        return Ok($"User logged in: {logindto.Username} (not really tho)");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CheckAuth()
    {
        return Ok("your are logged in as {user}");
    }
}