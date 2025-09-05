using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Infrastructure.Data;
using FootballStats.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FootballStats.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly AppDbContext _dbContext;
    
    public MatchRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IReadOnlyList<Match>> GetAsync(string? teamName1, string? teamName2, DateTime? fromDate, DateTime? toDate,
        int skip, int take)
    {
        var query = _dbContext.Matches
            .AsQueryable()
            .AsNoTracking();
        
        if (!string.IsNullOrEmpty(teamName1))
            query = query.Where(m => m.Team1 == teamName1);
        
        if (!string.IsNullOrEmpty(teamName2))
            query = query.Where(m => m.Team2 == teamName2);
        
        if (fromDate.HasValue)
            query = query.Where(m => m.Date >= fromDate.Value);
        
        if (toDate.HasValue)
            query = query.Where(m => m.Date <= toDate.Value);
        
        query = query.OrderByDescending(m => m.Date)
            .Skip(skip)
            .Take(take);
        
        return await query.ToListAsync();
    }
    
    public async Task<int> CountAsync(string? teamName1 = null, string? teamName2 = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _dbContext.Matches.AsQueryable();
        
        if (!string.IsNullOrEmpty(teamName1))
            query = query.Where(m => m.Team1 == teamName1);
        
        if (!string.IsNullOrEmpty(teamName2))
            query = query.Where(m => m.Team2 == teamName2);
        
        if (fromDate.HasValue)
            query = query.Where(m => m.Date >= fromDate.Value);
        
        if (toDate.HasValue)
            query = query.Where(m => m.Date <= toDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<string>> GetTeamNamesAsync()
    {
        var team1 = _dbContext.Matches
            .Select(m => m.Team1);
        
        var team2 = _dbContext.Matches
            .Select(m => m.Team2);
        
        var teams = await team1
            .Union(team2)
            .OrderBy(m => m)
            .ToListAsync();
        
        return teams;
    }
     
    public async Task AddAsync(Match matches)
    {
        await _dbContext.AddAsync(matches);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Matches
            .FindAsync(id);
    }
}