namespace CountryGwp.API.Services;

public interface IGwpService
{
    Task<Dictionary<string, decimal>> GetAverageGwpAsync(string country, IEnumerable<string> lobs);
}
