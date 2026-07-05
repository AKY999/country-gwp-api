using CountryGwp.API.Models;
using CountryGwp.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CountryGwp.API.Controllers;

[ApiController]
[Route("server/api/gwp/avg")]
public sealed class CountryGwpController : ControllerBase
{
    private readonly IGwpService _gwpService;

    public CountryGwpController(IGwpService gwpService)
    {
        _gwpService = gwpService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, decimal>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAverageGwpAsync([FromBody] GwpRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request body cannot be null.");
        }

        var result = await _gwpService.GetAverageGwpAsync(request.Country, request.Lob);
        return Ok(result);
    }
}
