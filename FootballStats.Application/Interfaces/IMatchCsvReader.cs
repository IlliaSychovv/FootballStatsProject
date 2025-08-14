using System.Text.RegularExpressions;
using FootballStats.Application.DTO;

namespace FootballStats.Application.Interfaces;

public interface IMatchCsvReader
{
    IAsyncEnumerable<MatchCsvModel> ReadAsync(Stream csvStream); 
}