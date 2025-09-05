using System.Globalization;
using CsvHelper;
using FootballStats.Application.DTO.Match;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
 
namespace FootballStats.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ExportService> _logger;

    public ExportService(AppDbContext dbContext, ILogger<ExportService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ExportToCsvAsync(Stream stream, CancellationToken token = default)
    { 
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteHeader<MatchExportDto>();
        await csv.NextRecordAsync();

        const int batchSize = 100;
        var batch = new List<MatchExportDto>(batchSize);

        await foreach (var match in _dbContext.Matches.AsNoTracking().AsAsyncEnumerable().WithCancellation(token))
        {
            batch.Add(match.Adapt<MatchExportDto>());
            
            if (batch.Count >= batchSize)
            {
                await WriteBatch(csv, batch, token);
                batch.Clear();
            }
        }
        
        if (batch.Count > 0)
            await WriteBatch(csv, batch, token);

        await writer.FlushAsync();
        _logger.LogInformation("CSV export completed");
    }
    
    private static async Task WriteBatch(CsvWriter csv, List<MatchExportDto> batch, CancellationToken token = default)
    {
        await csv.WriteRecordsAsync(batch, token);  
    }
}