using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Models.Auth;
using UserManagement.Application.Common.Models;
using Application.Common.Interfaces;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly ICurrentUserService _currentUserService;


        public AuthController(IAuthService authService, ILogger<AuthController> logger, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            _logger.LogInformation("recieve request to process login {@request}", request);

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                _logger.LogWarning("Failed login attempt for {Email}, Invalid email or password", request.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }
            _logger.LogInformation("User {Email} logged in successfully", response.Email);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (_currentUserService.UserId is null)
            {
                throw new UnauthorizedException("Unauthorized");
            }

            var userId = _currentUserService.UserId.Value;

            await _authService.RevokeAllTokensForUserAsync(userId);

            return Ok(new { message = "Logged out successfully." });
        }
        

        // [HttpPost("change-password")]
        // public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        // {
        //     var userId = _currentUserService.UserId;

        //     if (userId == null)
        //         throw new UnauthorizedException("User is not authenticated.");

        //     await _authService.ChangePasswordAsync(userId.Value, request);
        //     return Ok("Password changed successfully.");
        // }


    }
}
