namespace FootballStats.Application.DTO.Match;

public class MatchCsvModel
{
    public DateTime Date { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public string Score { get; set; }
}