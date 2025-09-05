namespace FootballStats.Application.Interfaces.Services;

public interface IExportService
{
    Task ExportToCsvAsync(Stream stream, CancellationToken token = default);
}