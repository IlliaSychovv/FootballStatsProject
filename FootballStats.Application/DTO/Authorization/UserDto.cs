namespace FootballStats.Application.DTO.Authorization;

public record UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Email { get; set; } 
    public string Login { get; set; } 
}