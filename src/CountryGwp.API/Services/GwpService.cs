using CountryGwp.API.Repositories;

namespace CountryGwp.API.Services;

public sealed class GwpService : IGwpService
{
    private readonly IGwpRepository _repository;

    public GwpService(IGwpRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dictionary<string, decimal>> GetAverageGwpAsync(string country, IEnumerable<string> lobs)
    {
        var records = await _repository.GetGwpDataAsync(country, lobs);
        var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var record in records)
        {
            var years = Enumerable.Range(2008, 8);
            var values = years
                .Select(year => record.YearValues.TryGetValue(year, out var val) ? val : null)
                .Where(val => val.HasValue)
                .Select(val => val!.Value)
                .ToList();

            if (values.Count > 0)
            {
                result[record.LineOfBusiness] = Math.Round(values.Average(), 1);
            }
        }

        return result;
    }
}
