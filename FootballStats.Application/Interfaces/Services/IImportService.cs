namespace FootballStats.Application.Interfaces.Services;

public interface IImportService
{ 
    Task ImportFromCsvAsync(Stream csvStream);
}