using FootballStats.Domain.Entity;

namespace FootballStats.Application.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<Match> GetByIdAsync(Guid id);
    Task AddAsync(Match matches);

    Task<IReadOnlyList<Match>> GetAsync(string? teamName1, string? teamName2, DateTime? fromDate, DateTime? toDate,
        int skip, int take);
    Task<int> CountAsync(string? teamName1 = null, string? teamName2 = null, DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<IReadOnlyList<string>> GetTeamNamesAsync();
}