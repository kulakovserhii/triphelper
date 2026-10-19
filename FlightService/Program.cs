using FlightService.Endpoints;
using FlightService.Providers;
using FlightService.Providers.Ignav;
using FlightService.Services;
using FlightService.Validators;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOptions<IgnavOptions>()
    .Bind(builder.Configuration.GetSection("Ignav"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "Ignav:ApiKey is not found")
    .ValidateOnStart();

builder.Services.AddHttpClient<IFlightProvider, IgnavFlightProvider>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<IgnavOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
    client.Timeout = TimeSpan.FromSeconds(20);
});


builder.Services.AddScoped<IFlightSearchService, FLightSearchService>();
builder.Services.AddValidatorsFromAssemblyContaining<SearchFlightsRequestValidator>();

builder.Services.AddSingleton<IAirportCatalog>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return InMemoryAirportCatalog.LoadFromFile(Path.Combine(env.ContentRootPath, "Data", "airports.json"));
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("search", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueLimit = 0;
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapFlightEndpoints();

app.MapGet("/api/airports/search", (string q, IAirportCatalog catalog) =>
    q.Trim().Length < 2
        ? Results.ValidationProblem(new Dictionary<string, string[]> { ["q"] = ["Введіть щонайменше 2 символи"] })
        : Results.Ok(catalog.Search(q)))
   .WithTags("Airports");


app.Run();