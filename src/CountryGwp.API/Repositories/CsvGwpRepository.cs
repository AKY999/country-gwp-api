using System.Globalization;
using CountryGwp.API.Models;

namespace CountryGwp.API.Repositories;

public sealed class CsvGwpRepository : IGwpRepository
{
    private readonly List<GwpRecord> _records = [];
    private readonly ILogger<CsvGwpRepository> _logger;

    public CsvGwpRepository(ILogger<CsvGwpRepository> logger)
    {
        _logger = logger;
        LoadCsvData();
    }

    private void LoadCsvData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "gwpByCountry.csv");
        if (!File.Exists(filePath))
        {
            _logger.LogError("CSV data file not found at: {FilePath}", filePath);
            throw new FileNotFoundException("Required database file is missing.", filePath);
        }

        try
        {
            using var reader = new StreamReader(filePath);
            var headerLine = reader.ReadLine();
            if (headerLine == null) return;

            while (reader.ReadLine() is { } line)
            {
                var parts = line.Split(',');
                if (parts.Length < 20)
                {
                    _logger.LogWarning("Skipped invalid CSV row: {Row}", line);
                    continue;
                }

                var country = parts[0].Trim();
                var variableId = parts[1].Trim();
                var variableName = parts[2].Trim();
                var lineOfBusiness = parts[3].Trim();

                var yearValues = new Dictionary<int, decimal?>();
                var isRowValid = true;

                for (int i = 0; i < 16; i++)
                {
                    int year = 2000 + i;
                    var valStr = parts[4 + i].Trim();
                    
                    if (string.IsNullOrEmpty(valStr))
                    {
                        yearValues[year] = null;
                    }
                    else if (decimal.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    {
                        yearValues[year] = val;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to parse GWP value '{Value}' for year {Year} in row: {Row}", valStr, year, line);
                        isRowValid = false;
                        break;
                    }
                }

                if (!isRowValid) continue;

                _records.Add(new GwpRecord
                {
                    Country = country,
                    VariableId = variableId,
                    VariableName = variableName,
                    LineOfBusiness = lineOfBusiness,
                    YearValues = yearValues
                });
            }

            _logger.LogInformation("Successfully loaded {Count} GWP records from CSV.", _records.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading CSV database.");
            throw;
        }
    }

    public Task<IEnumerable<GwpRecord>> GetGwpDataAsync(string country, IEnumerable<string> lobs)
    {
        var filtered = _records.Where(r => 
            r.Country.Equals(country, StringComparison.OrdinalIgnoreCase) &&
            lobs.Contains(r.LineOfBusiness, StringComparer.OrdinalIgnoreCase));

        return Task.FromResult(filtered);
    }
}
