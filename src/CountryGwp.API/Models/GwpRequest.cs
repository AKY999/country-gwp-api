using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CountryGwp.API.Models;

public sealed class GwpRequest
{
    [Required]
    [JsonPropertyName("country")]
    public required string Country { get; init; }

    [Required]
    [MinLength(1)]
    [JsonPropertyName("lob")]
    public required List<string> Lob { get; init; }
}
