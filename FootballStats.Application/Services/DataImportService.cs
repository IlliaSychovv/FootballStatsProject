using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Services;

namespace FootballStats.Application.Services;

public class DataImportService : IDataImportService
{
    private readonly IImportService _importService;

    public DataImportService(IImportService importService)
    {
        _importService = importService;
    }

    public async Task ImportMatches(Stream stream)
    {
        await _importService.ImportFromCsvAsync(stream);
    }
}