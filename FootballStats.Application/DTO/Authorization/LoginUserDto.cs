namespace FootballStats.Application.DTO.Authorization;

public record LoginUserDto
{
    public string Email { get; set; } 
    public string Password { get; set; } 
}