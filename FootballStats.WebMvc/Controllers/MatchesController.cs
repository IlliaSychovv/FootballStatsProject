using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FootballStats.WebMvc.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }
    
    [Authorize]
    public async Task<ActionResult> Index(string teamName1 = null, string? teamName2 = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var matches = await _matchService.GetAsync(teamName1, teamName2, fromDate, toDate);
        var paged = matches.Items;
        
        var teamNames = await _matchService.GetTeamNamesAsync();
        ViewBag.TeamNames = teamNames;
        
        return View(paged);
    }
}