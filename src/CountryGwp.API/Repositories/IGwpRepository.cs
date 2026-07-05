using CountryGwp.API.Models;

namespace CountryGwp.API.Repositories;

public interface IGwpRepository
{
    Task<IEnumerable<GwpRecord>> GetGwpDataAsync(string country, IEnumerable<string> lobs);
}
