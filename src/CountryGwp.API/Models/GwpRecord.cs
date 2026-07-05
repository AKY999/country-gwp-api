namespace CountryGwp.API.Models;

public sealed class GwpRecord
{
    public required string Country { get; init; }
    public required string VariableId { get; init; }
    public required string VariableName { get; init; }
    public required string LineOfBusiness { get; init; }
    public required Dictionary<int, decimal?> YearValues { get; init; }
}
