namespace FootballStats.Application.DTO;

public record PagedResponse<MatchDto>
{
    public List<MatchDto> Items { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}