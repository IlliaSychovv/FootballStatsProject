namespace FootballStats.Application.DTO.Authorization;

public record RegisterUserDto
{ 
    public string Email { get; set; } 
    public string Password { get; set; } 
}