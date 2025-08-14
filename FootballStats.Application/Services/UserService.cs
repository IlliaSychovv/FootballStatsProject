using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Domain.Entity;
using Mapster;

namespace FootballStats.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;

    public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
    }

    public async Task CreateUser(RegisterUserDto dto)
    {
        var user = dto.Adapt<User>();
        user.PasswordHash = _passwordHasherService.HashPassword(dto.Password);
        user.Id = Guid.NewGuid();
        
        await _userRepository.AddUserAsync(user);
    }

    public async Task<UserDto> GetUserLoginAsync(LoginUserDto dto)
    { 
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);

        bool isValid = _passwordHasherService.VerifyPassword(dto.Password, user.PasswordHash);
        if (!isValid) return null;

        return user.Adapt<UserDto>();
    }
}