using FootballStats.Application.DTO.Authorization;
using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballStats.WebMvc.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
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
        
        var user = await _userService.GetUserLoginAsync(model);
        if (user == null)
        {
            ModelState.AddModelError("", "Incorrect email or password");
            return View(model);
        }
        
        HttpContext.Session.SetString("Id", user.Id.ToString()); 
        
        return RedirectToAction("Index", "Matches");
    }
    
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserLogin");
        return RedirectToAction("Login");
    }
}