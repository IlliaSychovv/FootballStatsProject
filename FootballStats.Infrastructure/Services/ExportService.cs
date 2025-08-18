using System.Globalization;
using CsvHelper;
using FootballStats.Application.DTO.Match;
using FootballStats.Application.Interfaces.Services;
using FootballStats.Infrastructure.Data;
using FootballStats.Domain.Entity;
using Mapster;
using Microsoft.Extensions.Logging;
using ServiceStack.OrmLite;

namespace FootballStats.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly DbContext _context;
    private readonly ILogger<ExportService> _logger;

    public ExportService(DbContext context, ILogger<ExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExportToCsvAsync(Stream stream)
    {
        using var db = _context.Open();
        await using var writer = new StreamWriter(stream, leaveOpen: true);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteHeader<MatchExportDto>();
        await csv.NextRecordAsync();

        const int batchSize = 100;
        var batch = new List<MatchExportDto>(batchSize);

        foreach (var match in db.SelectLazy<Match>())
        {
            batch.Add(match.Adapt<MatchExportDto>());
            
            if (batch.Count >= batchSize)
            {
                WriteBatch(csv, batch);
                batch.Clear();
            }
        }
        
        if (batch.Count > 0)
            WriteBatch(csv, batch);

        await writer.FlushAsync();
        _logger.LogInformation("CSV export completed");
    }
    
    private static void WriteBatch(CsvWriter csv, List<MatchExportDto> batch)
    {
        foreach (var dto in batch)
        {
            csv.WriteRecord(dto);
            csv.NextRecord();
        }
    }
}
