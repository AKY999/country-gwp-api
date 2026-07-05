using Microsoft.Extensions.Caching.Memory;

namespace CountryGwp.API.Services;

public sealed class CachingGwpService : IGwpService
{
    private readonly IGwpService _innerService;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public CachingGwpService(IGwpService innerService, IMemoryCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }

    public async Task<Dictionary<string, decimal>> GetAverageGwpAsync(string country, IEnumerable<string> lobs)
    {
        var normalizedCountry = country.Trim().ToLowerInvariant();
        var sortedLobs = lobs.Select(l => l.Trim().ToLowerInvariant()).OrderBy(l => l).ToList();
        var cacheKey = $"gwp_avg_{normalizedCountry}_{string.Join("_", sortedLobs)}";

        if (_cache.TryGetValue<Dictionary<string, decimal>>(cacheKey, out var cachedResult))
        {
            return cachedResult!;
        }

        var result = await _innerService.GetAverageGwpAsync(country, lobs);

        _cache.Set(cacheKey, result, CacheDuration);

        return result;
    }
}
