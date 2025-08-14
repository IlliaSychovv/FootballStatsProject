using FootballStats.Application.DTO.Authorization;

namespace FootballStats.Application.Interfaces.Services;

public interface IUserService
{
    Task CreateUser(RegisterUserDto dto);
    Task<UserDto> GetUserLoginAsync(LoginUserDto dto);
}