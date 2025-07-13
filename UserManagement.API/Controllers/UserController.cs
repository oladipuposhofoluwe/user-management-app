using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest dto)
    {
        _logger.LogInformation("recieve request to create user");
        var user = await _userService.CreateUserAsync(dto);
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetUsers([FromQuery] UserQueryParameters parameters)
    {
        _logger.LogInformation("recieve request to list users");
        var result = await _userService.GetUsersAsync(parameters);
        return Ok(result);
    }

}