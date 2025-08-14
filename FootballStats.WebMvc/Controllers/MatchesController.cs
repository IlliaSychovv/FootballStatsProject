using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using FootballStats.WebMvc.Filters;

namespace FootballStats.WebMvc.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [SessionAuthorize]
    public async Task<ActionResult> Index(string teamName = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var matches = await _matchService.GetMatchesForMVC(teamName, fromDate, toDate);
        return View(matches);
    }
}