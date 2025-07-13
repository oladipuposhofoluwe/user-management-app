using Application.Common.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Models.Auth;



[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    private readonly ILogger<UsersController> _logger;
    public UsersController(IUserService userService, ILogger<UsersController> logger, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest dto)
    {
        Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@ 1");
        _logger.LogInformation("recieve request to create user");


        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid: {Errors}",
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }
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

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        _logger.LogInformation("recieve request to fetch user details");
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [Authorize(Roles = "Viewer")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {

        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("Unathurize");
        }

        _logger.LogInformation("recieve request to update user profile");
        await _userService.UpdateUserAsync(_currentUserService.UserId.Value, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteUser(Guid id)
    {
        _logger.LogInformation("recieve request to soft delete users");
        await _userService.SoftDeleteUserAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Staff")]
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        await _userService.DeactivateUserAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Viewer")]
    [HttpPatch("viewer/deactivate")]
    [Authorize]
    public async Task<IActionResult> SelfDeactivate()
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("Unathurize");
        }
        await _userService.DeactivateUserAsync(_currentUserService.UserId.Value);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Staff")]
    [HttpPatch("activate")]
    public async Task<IActionResult> AdminActivateAccount([FromQuery] Guid id)
    {
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("Unathurize");
        }
        await _userService.AdminActivateAccount(id);
        return NoContent();
    }

}