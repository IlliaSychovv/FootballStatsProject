using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using FootballStats.Application.DTO;
using FootballStats.Application.Interfaces;

namespace FootballStats.Infrastructure.Services;

public class CsvDataReader : IMatchCsvReader
{
    public async IAsyncEnumerable<MatchCsvModel> ReadAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });
        
        await foreach(var record in csv.GetRecordsAsync<MatchCsvModel>())
        {
            yield return record;
        }
    }
}