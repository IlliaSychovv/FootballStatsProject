using FootballStats.Application.Interfaces.Services;

namespace FootballStats.Application.Services;

public class DataExportService : IDataExportService
{
    private readonly IExportService _exportService;

    public DataExportService(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task ExportMatches(Stream outputStream)
    {
        await _exportService.ExportToCsvAsync(outputStream);
    }
}