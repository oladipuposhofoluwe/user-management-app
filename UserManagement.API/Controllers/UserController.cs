using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{

    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }



    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return Ok(user);
    }

}