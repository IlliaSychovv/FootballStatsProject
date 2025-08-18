using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballStats.WebMvc.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserAuthenticationService _authenticationService;

    public AccountController(IUserService userService, IUserAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
    }
    
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        await _userService.CreateUser(model);
        return RedirectToAction("Login");
    }
    
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto model)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        var login = await _authenticationService.SignInAsync(model);
        if (!login)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }
        
        return RedirectToAction("Index", "Matches");
    }
}