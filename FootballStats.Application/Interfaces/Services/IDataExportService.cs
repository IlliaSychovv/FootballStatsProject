namespace FootballStats.Application.Interfaces.Services;

public interface IDataExportService
{
    Task ExportMatches(Stream outputStream);
}