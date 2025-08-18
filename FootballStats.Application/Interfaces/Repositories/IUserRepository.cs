using FootballStats.Domain.Entity;

namespace FootballStats.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User> GetUserByLoginAsync(string email);
    Task<User> GetUserByEmailAsync(string email);
    Task<bool> IsUserExistAsync(string email);
}