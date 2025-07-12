using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Services;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            { 
                _logger.LogWarning("Failed login attempt for {Email}, Invalid email or password", request.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }
            _logger.LogInformation("User {Email} logged in successfully", response.Email);
            return Ok(response);
        }

    }
}
