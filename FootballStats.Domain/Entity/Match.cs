namespace FootballStats.Domain.Entity;

public class Match
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Team1 { get; set; } 
    public string Team2 { get; set; } 
    public string Score { get; set; } 
    
    public Match() { }
}