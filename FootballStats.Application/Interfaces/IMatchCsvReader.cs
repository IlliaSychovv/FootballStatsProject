using FootballStats.Application.DTO.Match;

namespace FootballStats.Application.Interfaces;

public interface IMatchCsvReader
{
    IAsyncEnumerable<MatchCsvModel> ReadAsync(Stream csvStream); 
}