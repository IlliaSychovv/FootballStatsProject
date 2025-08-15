using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using FootballStats.Domain.Entity;
using ServiceStack.OrmLite;

namespace FootballStats.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly IMatchCsvReader _reader;
    private readonly DbContext _context;
    private readonly ILogger<ImportService> _logger;

    public ImportService(
        IMatchCsvReader reader,
        DbContext context,
        ILogger<ImportService> logger)
    {
        _reader = reader;
        _context = context;
        _logger = logger;
    }

    public async Task ImportFromCsvAsync(Stream csvStream)
    {
        const int batchSize = 100;
        var batch = new List<Match>();

        using var db = _context.Open();
        using var transaction = db.OpenTransaction();

        try
        {
            var existingMatches = (await db.SelectAsync<Match>())
                .Select(m => (m.Date, m.Team1, m.Team2))
                .ToHashSet();

            int currentId = 500;

            await foreach (var csvMatch in _reader.ReadAsync(csvStream))
            {
                if (csvMatch == null || string.IsNullOrWhiteSpace(csvMatch.Team1) || string.IsNullOrWhiteSpace(csvMatch.Team2))
                {
                    _logger.LogWarning("Skipping invalid CSV record");
                    continue;
                }

                if (existingMatches.Contains((csvMatch.Date, csvMatch.Team1, csvMatch.Team2)))
                {
                    _logger.LogInformation("Skipping duplicate match: {Date} {Team1} vs {Team2}",
                        csvMatch.Date, csvMatch.Team1, csvMatch.Team2);
                    continue;
                }

                batch.Add(new Match
                {
                    Id = currentId++,
                    Date = csvMatch.Date,
                    Team1 = csvMatch.Team1,
                    Team2 = csvMatch.Team2,
                    Score = csvMatch.Score
                });

                if (batch.Count >= batchSize)
                {
                    await db.InsertAllAsync(batch);
                    _logger.LogInformation("Imported batch of {Count} matches", batch.Count);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await db.InsertAllAsync(batch);
                _logger.LogInformation("Imported final batch of {Count} matches", batch.Count);
            }

            transaction.Commit();
            _logger.LogInformation("CSV import completed successfully.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error during CSV import. Transaction rolled back.");
            throw;
        }
    }
}