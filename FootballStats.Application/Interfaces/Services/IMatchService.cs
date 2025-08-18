using FootballStats.Application.DTO;
using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.DTO.Match;

namespace FootballStats.Application.Interfaces.Services;

public interface IMatchService
{
    Task<MatchDto> GetMatchByIdAsync(Guid matchId); 
    Task<MatchDto> AddMatchAsync(CreateMatchDto createMatchDto); 
    Task<PagedResponse<MatchDto>> GetAsync(string? teamName1 = null, string? teamName2 = null, DateTime? fromDate = null, DateTime? toDate = null, 
        int pageNumber = 1, int pageSize = 50);
    Task<IReadOnlyList<string>> GetTeamNamesAsync();
}