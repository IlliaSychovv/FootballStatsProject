using FootballStats.Application.DTO;
using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.DTO.Match;
using FootballStats.Domain.Entity;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Interfaces.Services;
using Mapster;
using SequentialGuid;

namespace FootballStats.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _repository;

    public MatchService(IMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<MatchDto> GetMatchByIdAsync(Guid matchId)
    {
        var match = await _repository.GetByIdAsync(matchId);
        return match.Adapt<MatchDto>();
    }

    public async Task<MatchDto> AddMatchAsync(CreateMatchDto createMatchDto)
    {
        var match = createMatchDto.Adapt<Match>();
        match.Id = SequentialGuidGenerator.Instance.NewGuid();
        
        await _repository.AddAsync(match);
        return match.Adapt<MatchDto>();
    }
    
    public async Task<PagedResponse<MatchDto>> GetAsync(string? teamName1 = null, string? teamName2 = null, DateTime? fromDate = null, DateTime? toDate = null,
        int pageNumber = 1, int pageSize = 50)
    {
        int skip = (pageNumber - 1) * pageSize;
        int take = pageSize;

        var matches = await _repository.GetAsync(teamName1, teamName2, fromDate, toDate, skip, take);
        var totalCount = await _repository.CountAsync(teamName1, teamName2, fromDate, toDate);

        var items = matches.Adapt<List<MatchDto>>();

        return new PagedResponse<MatchDto>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public async Task<IReadOnlyList<string>> GetTeamNamesAsync()
    {
        return await _repository.GetTeamNamesAsync();
    }
}