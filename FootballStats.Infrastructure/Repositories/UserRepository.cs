using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Domain.Entity;
using FootballStats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballStats.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    
    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<User?> GetUserByLoginAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsUserExistAsync(string email)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email == email);
    }
}