namespace FootballStats.Application.Interfaces.Services;

public interface IDataImportService
{
    Task ImportMatches(Stream stream);
}