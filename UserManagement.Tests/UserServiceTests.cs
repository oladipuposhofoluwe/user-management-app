using Xunit;
using Moq;
using FluentAssertions;
using UserManagement.Domain.Entities;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Common.Exceptions;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHash> _passwordHashMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _passwordHashMock = new Mock<IPasswordHash>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _userService = new UserService(
            _userRepoMock.Object,
            _passwordHashMock.Object,
            _loggerMock.Object
        );
    }

    // [Fact]
    // public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
    // {
    //     // Arrange
    //     var email = "test@example.com";
    //     var expectedUser = new User { Email = email };
    //     _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(expectedUser);

    //     // Act
    //     var result = await _userService.GetByEmailAsync(email);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result.Email.Should().Be(email);
    // }

    // [Fact]
    // public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    // {
    //     // Arrange
    //     var email = "notfound@example.com";
    //     _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

    //     // Act
    //     var result = await _userService.GetByEmailAsync(email);

    //     // Assert
    //     result.Should().BeNull();
    // }
    


    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser_WhenEmailIsUnique()
    {
    
        var request = new RegisterRequest
        {
            FirstName = "oladipupo",
            LastName = "shofoluwe",
            Email = "oladipupo.shofoluwe@gmail.com",
            PhoneNumber = "08067644805",
            Password = "Password123!",
            Role = Role.Staff
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email))
                        .ReturnsAsync((User)null!);

        _passwordHashMock.Setup(x => x.HashPassword(request.Password))
                            .Returns("hashed_password");

        _userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>()))
                        .Returns(Task.CompletedTask);

        var result = await _userService.CreateUserAsync(request);

        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Role.Should().Be("Staff");

        _userRepoMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Email == request.Email)), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowConflictException_WhenEmailExists()
    {
        var request = new RegisterRequest
        {
            FirstName = "oladipupo",
            LastName = "shofoluwe",
            Email = "oladipupo.shofoluwe@gmail.com",
            PhoneNumber = "08067644805",
            Password = "Secret!",
            Role = Role.Admin
        };

        var existingUser = new User { Email = request.Email };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email))
                        .ReturnsAsync(existingUser);

        Func<Task> act = async () => await _userService.CreateUserAsync(request);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A user with this email already exists.");
    }
}

