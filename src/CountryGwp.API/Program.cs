using CountryGwp.API.Middleware;
using CountryGwp.API.Repositories;
using CountryGwp.API.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(9091);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IGwpRepository, CsvGwpRepository>();
builder.Services.AddScoped<GwpService>();
builder.Services.AddScoped<IGwpService>(provider =>
    new CachingGwpService(
        provider.GetRequiredService<GwpService>(),
        provider.GetRequiredService<IMemoryCache>()
    ));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Country GWP API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

app.Run();
