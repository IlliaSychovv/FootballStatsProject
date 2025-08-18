using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Infrastructure.Data;
using ServiceStack.OrmLite;
using FootballStats.Domain.Entity;

namespace FootballStats.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly DbContext _context;

    public MatchRepository(DbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyList<Match>> GetAsync(string? teamName1, string? teamName2, DateTime? fromDate, DateTime? toDate,
        int skip, int take)
    {
        using var db = _context.Open();
        
        var q = db.From<Match>();

        if (!string.IsNullOrEmpty(teamName1))
            q = q.Where(m => m.Team1 == teamName1);
        
        if (!string.IsNullOrEmpty(teamName2))
            q = q.Where(m => m.Team2 == teamName2);
        
        if (fromDate.HasValue)
            q = q.Where(m => m.Date >= fromDate.Value);
        
        if (toDate.HasValue)
            q = q.Where(m => m.Date <= toDate.Value);
        
        q = q.OrderByDescending(m => m.Date)
            .Limit(skip, take);
        
        return await db.SelectAsync(q);
    }
    
    public async Task<int> CountAsync(string? teamName1 = null, string? teamName2 = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        using var db = _context.Open();

        var q = db.From<Match>();

        if (!string.IsNullOrEmpty(teamName1))
            q = q.Where(m => m.Team1 == teamName1);
        
        if (!string.IsNullOrEmpty(teamName2))
            q = q.Where(m => m.Team2 == teamName2);

        if (fromDate.HasValue)
            q = q.Where(m => m.Date >= fromDate.Value);

        if (toDate.HasValue)
            q = q.Where(m => m.Date <= toDate.Value);

        return (int)await db.CountAsync(q);
    }

    public async Task<IReadOnlyList<string>> GetTeamNamesAsync()
    {
        using var db = _context.Open();

        var matches = await db.SelectAsync<Match>();
        
        var teams = matches
            .Select(m => m.Team1)
            .Union(matches.Select(m => m.Team2))
            .OrderBy(m => m)
            .ToList();
        
        return teams;
    }
     
    public async Task AddAsync(Match matches)
    {
        using var db = _context.Open();
        await db.InsertAsync(matches);
    }

    public async Task<Match> GetByIdAsync(Guid id)
    {
        using var db = _context.Open();
        return await db.SingleByIdAsync<Match>(id);
    }
}