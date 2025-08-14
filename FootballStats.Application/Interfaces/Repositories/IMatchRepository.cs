using FootballStats.Domain.Entity;

namespace FootballStats.Application.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<Match> GetByIdAsync(int id);
    Task AddAsync(Match matches);
    Task<IReadOnlyList<Match>> GetAsync(string? teamName, DateTime? fromDate, DateTime? toDate, 
        int skip, int take);
    Task<int> CountAsync(string? teamName = null, DateTime? fromDate = null, DateTime? toDate = null);
}