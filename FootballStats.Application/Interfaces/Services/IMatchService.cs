using FootballStats.Application.DTO;

namespace FootballStats.Application.Interfaces.Services;

public interface IMatchService
{
    Task<MatchDto> GetMatchByIdAsync(int matchId);
    Task<MatchDto> AddMatchAsync(MatchDto matchDto);
    Task<IReadOnlyList<MatchDto>> GetMatchesForMVC(string? teamName, DateTime? fromDate, DateTime? toDate);
    Task<PagedResponse<MatchDto>> GetAsync(string? teamName = null, DateTime? fromDate = null, DateTime? toDate = null, 
        int pageNumber = 1, int pageSize = 50);
}