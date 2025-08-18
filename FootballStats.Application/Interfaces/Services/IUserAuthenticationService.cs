using FootballStats.Application.DTO.Authorization;

namespace FootballStats.Application.Interfaces.Services;

public interface IUserAuthenticationService
{
    Task<bool> SignInAsync(LoginUserDto loginDto);
}