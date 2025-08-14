using FootballStats.Application.DTO;
using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballStats.WebApi.Controllers;

[ApiController]
[Route("api/v1/matches")]
public class MatchController : ControllerBase
{
    private readonly IMatchService _matchService;
    private readonly IDataImportService _dataImportService;
    private readonly IDataExportService _exportService;

    public MatchController(IMatchService matchService, IDataImportService dataImportService, IDataExportService exportService)
    {
        _matchService = matchService;
        _dataImportService = dataImportService;
        _exportService = exportService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMatch(int id)
    {
        var match = await _matchService.GetMatchByIdAsync(id);
        return Ok(match);
    }

    [HttpPost]
    public async Task<IActionResult> AddMatch([FromBody] MatchDto dto)
    {
        var match = await _matchService.AddMatchAsync(dto);
        return CreatedAtAction(nameof(GetMatch), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<MatchDto>>> GetAllMatches([FromQuery] string? teamName = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var matches = await _matchService.GetAsync(teamName, fromDate, toDate, pageNumber, pageSize);
        
        return Ok(matches);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        await _dataImportService.ImportMatches(file.OpenReadStream());
        return Ok("Imported!");
    }

    [HttpGet("export")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportMatches()
    {
        var stream = new MemoryStream();
        await _exportService.ExportMatches(stream);
        stream.Position = 0;

        return File(stream, "text/csv", "matches_export.csv");
    }
}