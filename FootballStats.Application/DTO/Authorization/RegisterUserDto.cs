namespace FootballStats.Application.DTO.Authorization;

public record RegisterUserDto
{
    public string Name { get; set; } 
    public string Email { get; set; } 
    public string Login { get; set; } 
    public string Password { get; set; } 
}