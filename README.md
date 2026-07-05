# CountryGwp API

Self-hosted ASP.NET Core Web API that returns average Gross Written Premium (GWP) for a given country and lines of business over the 2008–2015 period.

## Requirements

- .NET 8.0 SDK

## Running the API

```bash
dotnet run --project src/CountryGwp.API
```

Server starts on `http://localhost:9091`.

## Endpoint

**POST** `http://localhost:9091/server/api/gwp/avg`

Request body:
```json
{
    "country": "ae",
    "lob": ["property", "transport"]
}
```

- `country` — ISO 2-letter country code (e.g. `us`, `gb`, `de`, `ae`)
- `lob` — one or more lines of business: `transport`, `freight`, `property`, `liability`, `motor`, `life`, `a_s`, `other`, `pecuniary_loss`

Example response:
```json
{
    "transport": 285137382.5,
    "property": 599026844.9
}
```

### curl

```bash
curl -X POST http://localhost:9091/server/api/gwp/avg \
     -H "Content-Type: application/json" \
     -d '{"country": "ae", "lob": ["property", "transport"]}'
```

### PowerShell

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:9091/server/api/gwp/avg" -ContentType "application/json" -Body '{"country": "ae", "lob": ["property", "transport"]}'
```

## Swagger

Once running, open `http://localhost:9091/swagger` to test interactively.

## Tests

```bash
dotnet test
```
