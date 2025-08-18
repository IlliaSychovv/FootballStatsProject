using FootballStats.Application.DTO;
using FootballStats.Application.DTO.Match;
using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballStats.WebApi.Controllers;

[ApiController]
[Route("api/v1/matches")]
public class MatchController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly IExportService _exportService;
    private readonly IImportService _importService;

    public MatchController(IMatchService matchService, IExportService exportService, IImportService importService)
    {
        _matchService = matchService;
        _exportService = exportService;
        _importService = importService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMatch(Guid id)
    {
        var match = await _matchService.GetMatchByIdAsync(id);
        return Ok(match);
    }

    [HttpPost]
    public async Task<IActionResult> AddMatch([FromBody] CreateMatchDto dto)
    {
        var match = await _matchService.AddMatchAsync(dto);
        return Created(string.Empty, match); 
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<MatchDto>>> GetAllMatches([FromQuery] string? teamName1 = null,
        [FromQuery] string? teamName2 = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var matches = await _matchService.GetAsync(teamName1, teamName2, fromDate, toDate, pageNumber, pageSize);
        
        return Ok(matches);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        await _importService.ImportFromCsvAsync(file.OpenReadStream());
        return Ok("Imported!");
    }

    [HttpGet("export")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportMatches()
    {
        var stream = new MemoryStream();
        await _exportService.ExportToCsvAsync(stream);
        stream.Position = 0;
        
        return File(stream, "text/csv", "matches_export.csv");
    }
}