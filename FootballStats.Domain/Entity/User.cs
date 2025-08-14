namespace FootballStats.Domain.Entity;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Email { get; set; } 
    public string Login { get; set; } 
    public string PasswordHash { get; set; } 
}