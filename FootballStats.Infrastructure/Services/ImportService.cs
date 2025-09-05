using FootballStats.Application.Interfaces;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using FootballStats.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FootballStats.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly IMatchCsvReader _reader;
    private readonly AppDbContext _dbContexts;
    private readonly ILogger<ImportService> _logger;

    public ImportService(
        IMatchCsvReader reader,
        AppDbContext dbContexts,
        ILogger<ImportService> logger)
    {
        _reader = reader;
        _dbContexts = dbContexts;
        _logger = logger;
    }

    public async Task ImportFromCsvAsync(Stream csvStream, CancellationToken token = default)
    {
        const int batchSize = 100;
        var batch = new List<Match>();
        
        var existingMatches = new HashSet<(DateTime Date, string Team1, string Team2)>();
        
        await using var transaction = await _dbContexts.Database.BeginTransactionAsync(token);
        
        try
        {
            await foreach (var csvMatch in _reader.ReadAsync(csvStream).WithCancellation(token))
            {
                if (csvMatch == null || string.IsNullOrWhiteSpace(csvMatch.Team1) || string.IsNullOrWhiteSpace(csvMatch.Team2))
                {
                    _logger.LogWarning("Skipping import because CSV file is empty or null");
                    continue;
                }
                
                var matchDateUtc = DateTime.SpecifyKind(csvMatch.Date, DateTimeKind.Utc);

                if (matchDateUtc > DateTime.UtcNow)
                {
                    _logger.LogInformation("Skipping match with future date {Date}, {Team1}, {Team2}",
                        matchDateUtc, csvMatch.Team1, csvMatch.Team2);    
                    continue;
                }

                if (existingMatches.Contains((matchDateUtc, csvMatch.Team1, csvMatch.Team2)))
                {
                    _logger.LogInformation("Skipping duplicate match {Date}, {Team1}, {Team2}",
                        matchDateUtc, csvMatch.Team1, csvMatch.Team2);
                    continue;
                }

                bool exists = await _dbContexts.Matches
                    .AnyAsync(m => m.Date == matchDateUtc &&
                                   m.Team1 == csvMatch.Team1 &&
                                   m.Team2 == csvMatch.Team2, token);
                
                if (exists)
                {
                    _logger.LogInformation("Skipping duplicate from DB {Date}, {Team1}, {Team2}",
                        matchDateUtc, csvMatch.Team1, csvMatch.Team2);
                    continue;
                }

                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    Date = matchDateUtc,
                    Team1 = csvMatch.Team1,
                    Team2 = csvMatch.Team2,
                    Score = csvMatch.Score
                };
                
                batch.Add(match);
                existingMatches.Add((csvMatch.Date, csvMatch.Team1, csvMatch.Team2));

                if (batch.Count >= batchSize)
                {
                    await _dbContexts.Matches.AddRangeAsync(batch, token);
                    await _dbContexts.SaveChangesAsync(token);
                    batch.Clear();
                }
            }
            
            if (batch.Count > 0)
            {
                await _dbContexts.Matches.AddRangeAsync(batch, token);
                await _dbContexts.SaveChangesAsync(token);
                _logger.LogInformation("Imported last {MatchCount} matches", batch.Count);
            }
                
            await transaction.CommitAsync(token);
            _logger.LogInformation("Import of CSV file competed");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            _logger.LogError(ex, "An error occured while importing from csv file.");
            throw;
        }
    }
}