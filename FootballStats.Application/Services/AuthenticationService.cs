using System.Security.Claims;
using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace FootballStats.Application.Services;

public class AuthenticationService : IUserAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> SignInAsync(LoginUserDto loginDto)
    {
        var user = await _userService.GetUserLoginAsync(loginDto);
        if (user == null)
            return false;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies"); 
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };
        
        await _httpContextAccessor.HttpContext!.SignInAsync(
            "Cookies", 
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        
        return true;
    }
}