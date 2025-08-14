using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Domain.Entity;
using FootballStats.Infrastructure.Data;
using ServiceStack.OrmLite;

namespace FootballStats.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user)
    {
        using var db = _context.Open();
        await db.InsertAsync(user);
    }

    public async Task<User> GetUserByLoginAsync(string login)
    {
        using var db = _context.Open();
        return await db.SingleAsync<User>(u => u.Login == login);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        using var db = _context.Open();
        return await db.SingleAsync<User>(u => u.Email == email);
    }

    public async Task<bool> IsUserExistAsync(string login)
    {
        using var db = _context.Open();
        return await db.ExistsAsync<User>(u => u.Login == login);
    }
}