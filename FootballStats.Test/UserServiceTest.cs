using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Services;
using FootballStats.Domain.Entity;
using Moq;
using Shouldly;

namespace FootballStats.Test;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasherService = new Mock<IPasswordHasherService>();
        _userService = new UserService(_mockUserRepository.Object, _mockPasswordHasherService.Object);
    }
    
    [Fact]
    public async Task CreateUser_ShouldCreateUser_WhenWeCallMethod()
    {
        var dto = new RegisterUserDto
        {
            Email = "test@test.com",
            Password = "test123"
        };
        
        _mockUserRepository
            .Setup(x => x.AddUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        
        _mockPasswordHasherService
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(dto.Password);

        await _userService.CreateUser(dto);
        
        _mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
        _mockPasswordHasherService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetUserLoginAsync_ShouldReturnUser_WhenUserExists()
    {
        var dto = new LoginUserDto
        {
            Email = "test@test.com",
            Password = "test123"
        };

        var user = new User
        {   
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = "test12345"
        };
        
        _mockUserRepository
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        
        _mockPasswordHasherService
            .Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash))
            .Returns(true);
        
        var result = await _userService.GetUserLoginAsync(dto);
        
        result.ShouldNotBeNull();
        result.Email.ShouldBe(dto.Email);
        
        _mockUserRepository.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockPasswordHasherService.Verify(x => x.VerifyPassword(dto.Password, user.PasswordHash), Times.Once);
    }
    
    [Fact]
    public async Task GetUserLoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        var dto = new LoginUserDto
        {
            Email = "test@test.com",
            Password = "wrongPassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = "test12345"
        };

        _mockUserRepository
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockPasswordHasherService
            .Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash))
            .Returns(false);  

        var result = await _userService.GetUserLoginAsync(dto);

        result.ShouldBeNull();

        _mockUserRepository.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Once);
        _mockPasswordHasherService.Verify(x => x.VerifyPassword(dto.Password, user.PasswordHash), Times.Once);
    }

}