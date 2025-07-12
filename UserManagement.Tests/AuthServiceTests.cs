using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserManagement.Infrastructure.Services;
using UserManagement.Domain.Entities;
using UserManagement.Application.Common.Models.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890abcdEFGH");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        _authService = new AuthService(_userRepoMock.Object, _configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "oladipupo.shofoluwe@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = Role.Admin
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "password123"
        };

        var result = await _authService.LoginAsync(request);

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be(user.Role.ToString());
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("missing@example.com")).ReturnsAsync((User?)null);

        var request = new LoginRequest
        {
            Email = "missing@example.com",
            Password = "any"
        };

        Func<Task> act = async () => await _authService.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsIncorrect()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "oladipupo.shofoluwe@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            Role = Role.Viewer
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "wrong-password"
        };

        Func<Task> act = async () => await _authService.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials.");
    }
}
