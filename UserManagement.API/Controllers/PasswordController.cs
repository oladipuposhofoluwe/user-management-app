using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {

        private readonly IPasswordService _passwordService;
        private readonly ILogger<PasswordController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PasswordController(IPasswordService passwordService, ILogger<PasswordController> logger, ICurrentUserService currentUserService)
        {
            _passwordService = passwordService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _passwordService.ForgotPasswordAsync(request.Email);
            return Ok("Password reset link has been sent if the email exists.");
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _passwordService.ResetPasswordAsync(request);
            return Ok("Password has been reset successfully.");
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
                throw new UnauthorizedException("User is not authenticated.");

            await _passwordService.ChangePasswordAsync(userId.Value, request);
            return Ok("Password changed successfully.");
        }



    }
}