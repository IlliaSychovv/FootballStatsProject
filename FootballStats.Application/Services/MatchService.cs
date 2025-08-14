using FootballStats.Application.DTO;
using FootballStats.Domain.Entity;
using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Repositories;
using FootballStats.Application.Interfaces.Services;
using Mapster;

namespace FootballStats.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _repository;

    public MatchService(IMatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<MatchDto> GetMatchByIdAsync(int matchId)
    {
        var match = await _repository.GetByIdAsync(matchId);
        return match.Adapt<MatchDto>();
    }

    public async Task<MatchDto> AddMatchAsync(MatchDto matchDto)
    {
        var match = matchDto.Adapt<Match>();
        await _repository.AddAsync(match);
        return match.Adapt<MatchDto>();
    }

    public async Task<IReadOnlyList<MatchDto>> GetMatchesForMVC(string? teamName, DateTime? fromDate, DateTime? toDate)
    {
        var matches = await _repository.GetAsync(teamName, fromDate, toDate, 0, 50);
        var matchesDto = matches.Adapt<List<MatchDto>>();
        return matchesDto;
    }
    
    public async Task<PagedResponse<MatchDto>> GetAsync(string? teamName = null, DateTime? fromDate = null, DateTime? toDate = null,
        int pageNumber = 1, int pageSize = 50)
    {
        int skip = (pageNumber - 1) * pageSize;
        int take = pageSize;

        var matches = await _repository.GetAsync(teamName, fromDate, toDate, skip, take);
        var totalCount = await _repository.CountAsync(teamName, fromDate, toDate);

        var items = matches.Adapt<List<MatchDto>>();

        return new PagedResponse<MatchDto>
        {
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}